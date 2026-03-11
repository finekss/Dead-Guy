# Система оружия — Техническая документация

## Содержание

1. [Обзор архитектуры](#1-обзор-архитектуры)
2. [Как работает система](#2-как-работает-система)
3. [Как создать новое оружие](#3-как-создать-новое-оружие)
4. [Настройка WeaponData](#4-настройка-weapondata)
5. [Настройка префабов](#5-настройка-префабов)
6. [Подключение системы в Unity](#6-подключение-системы-в-unity)
7. [Настройка WeaponPivot](#7-настройка-weaponpivot)
8. [Тестирование системы на сцене](#8-тестирование-системы-на-сцене)
9. [Отладка типичных проблем](#9-отладка-типичных-проблем)
10. [Рекомендации по улучшению](#10-рекомендации-по-улучшению)

---

## 1. Обзор архитектуры

### Паттерны

Система оружия построена на базе **MVP (Model–View–Presenter)** и следует принципам **SOLID**, **композиция важнее наследования**, **низкая связанность**.

### Структура файлов

```
Weapon/
├── IWeapon.cs                  — интерфейс оружия
├── IDamageable.cs              — интерфейс получателя урона
├── WeaponData.cs               — ScriptableObject с данными оружия
├── WeaponModel.cs              — модель (рантайм-состояние)
├── WeaponView.cs               — вью (визуалы, спавн снарядов)
├── WeaponPresenter.cs          — презентер (логика, оркестрация)
├── WeaponInventory.cs          — инвентарь оружия игрока
│
├── Behaviors/
│   ├── IFiringBehavior.cs      — интерфейс поведения стрельбы
│   ├── FiringBehaviorFactory.cs — фабрика поведений
│   ├── SemiAutoFiringBehavior.cs
│   ├── AutomaticFiringBehavior.cs
│   ├── ChargedFiringBehavior.cs
│   ├── MeleeFiringBehavior.cs
│   └── BeamFiringBehavior.cs
│
├── Projectiles/
│   ├── Projectile.cs           — компонент снаряда
│   └── ProjectilePool.cs       — пул объектов для снарядов
│
└── Pickup/
    ├── WorldWeapon.cs           — оружие в мире (можно подобрать)
    └── WeaponPickupSystem.cs    — система подбора оружия рейкастом
```

### Диаграмма классов

```
              ┌──────────────────┐
              │   WeaponData     │ (ScriptableObject)
              │   (конфигурация) │
              └──────┬───────────┘
                     │
          ┌──────────▼──────────┐
          │   WeaponPresenter   │ implements IWeapon
          │     (логика)        │
          ├─────────────────────┤
          │ - WeaponModel       │ (рантайм-состояние: патроны, перезарядка)
          │ - WeaponView        │ (MonoBehaviour на префабе оружия)
          │ - IFiringBehavior   │ (стратегия стрельбы)
          └──────────┬──────────┘
                     │
     ┌───────────────▼───────────────┐
     │       WeaponInventory          │
     │  (2 слота, активное оружие)   │
     └───────────────┬───────────────┘
                     │
         ┌───────────▼───────────┐
         │  CharacterPresenter   │
         │  (делегирует команды) │
         └───────────┬───────────┘
                     │
         ┌───────────▼───────────┐
         │   UnityInputSystem    │
         │  (ввод: атака, E, R, Q)│
         └───────────────────────┘
```

### Ответственности классов

| Класс | Ответственность |
|-------|----------------|
| **WeaponData** | Хранение всех параметров оружия (ScriptableObject). Дизайнер создаёт новое оружие без кода. |
| **WeaponModel** | Рантайм-состояние оружия: текущие патроны, перезарядка, расчёт урона. |
| **WeaponView** | MonoBehaviour на префабе. Спавн снарядов, hitscan, melee overlap, визуальные эффекты. |
| **WeaponPresenter** | Оркестрирует Model и View. Определяет КОГДА и КАК стрелять. Реализует IWeapon. |
| **IFiringBehavior** | Стратегия обработки ввода: полуавтомат, автомат, заряжённый, ближний бой, луч. |
| **FiringBehaviorFactory** | Создаёт нужное IFiringBehavior по FiringMode из WeaponData. |
| **WeaponInventory** | Управляет 2 слотами оружия, подбором, выбрасыванием, переключением. |
| **WeaponPickupSystem** | Рейкаст из центра экрана, обнаружение WorldWeapon, подбор по нажатию E. |
| **WorldWeapon** | MonoBehaviour на объекте оружия в мире. Вращение, покачивание, логика подбора. |
| **Projectile** | Летящий снаряд. Наносит урон при столкновении, самоуничтожается по таймеру. |
| **ProjectilePool** | Пул объектов для снарядов (оптимизация аллокаций). |

---

## 2. Как работает система

### Жизненный цикл оружия

1. **Оружие в мире** — `WorldWeapon` лежит на сцене с привязанным `WeaponData`
2. **Рейкаст** — `WeaponPickupSystem` каждый кадр стреляет рейкастом из центра камеры
3. **Подбор (E)** — игрок нажимает E, вызывается `TryPickup()`:
   - Если есть пустой слот → оружие занимает пустой слот
   - Если оба слота заняты → активное оружие выбрасывается, новое занимает слот
4. **Создание оружия** — `WeaponInventory.CreateWeaponInSlot()`:
   - Инстанциируется `weaponPrefab` из WeaponData
   - Создаётся `WeaponModel` с данными из WeaponData
   - Создаётся `WeaponPresenter` с model + view + поведением стрельбы
5. **Атака** — ввод `StartAttack`/`StopAttack` через `IControllable` → `CharacterPresenter` → `WeaponInventory.ActiveWeapon`:
   - `WeaponPresenter` вызывает `IFiringBehavior.OnAttackPressed/Released/Tick`
   - Behavior определяет тайминг, вызывает `presenter.ExecuteShot/ExecuteMelee/etc.`
   - Presenter вызывает нужный метод View (спавн снаряда, рейкаст, melee overlap, луч)
6. **Перезарядка (R)** — `WeaponModel.StartReload()`, автоматическое завершение через `TryFinishReload()` в `Tick()`
7. **Переключение (Q)** — `WeaponInventory.SwitchWeapon()`, текущее оружие скрывается, другое показывается

### Система поведений (Strategy Pattern)

| FiringMode | Поведение | Тип урона |
|-----------|-----------|-----------|
| SemiAuto | Один выстрел за нажатие | Projectile / Hitscan |
| Automatic | Стрельба пока зажата кнопка | Projectile / Hitscan |
| Charged | Зарядка при зажатии, выстрел при отпускании | Projectile / Hitscan |
| Melee | Удар при нажатии (OverlapSphere) | Melee |
| Beam | Луч пока зажата кнопка (LineRenderer + Raycast) | Beam |

### Типы нанесения урона (DamageType)

| DamageType | Как работает |
|-----------|-------------|
| Projectile | Спавн GameObject с компонентом `Projectile`, летит по направлению, наносит урон при столкновении |
| Hitscan | Мгновенный Raycast от дула, наносит урон первому попавшемуся |
| Melee | OverlapSphere + проверка угла от дула, наносит урон всем в зоне |
| Beam | Непрерывный Raycast + LineRenderer, наносит урон каждые 0.1с |

---

## 3. Как создать новое оружие

### Шаг 1: Создать WeaponData

1. В Unity: **ПКМ в Project → Create → Game → Weapon**
2. Назовите файл (например, `Shotgun_Data`)
3. Заполните поля (см. раздел 4)

### Шаг 2: Создать префаб оружия (weaponPrefab)

Это модель оружия, которая появляется в руках игрока:

1. Создайте пустой GameObject
2. Добавьте 3D-модель оружия как дочерний объект
3. Добавьте компонент `WeaponView`
4. Настройте ссылки на `WeaponView`:
   - **Muzzle Point** — Transform на конце ствола (для спавна снарядов)
   - **Melee Hit Point** — Transform для ближнего боя (опционально)
   - **Beam Renderer** — LineRenderer для лучевого оружия (опционально)
   - **Muzzle Flash** — ParticleSystem для вспышки (опционально)
5. Сохраните как префаб

### Шаг 3: Создать префаб снаряда (projectilePrefab) — если нужен

1. Создайте GameObject со сферой/капсулой или своей моделью
2. Добавьте **Rigidbody** (Use Gravity = false)
3. Добавьте **Collider** (Is Trigger = true)
4. Добавьте компонент `Projectile` (из namespace `Weapon.Projectiles`)
5. Сохраните как префаб

### Шаг 4: Создать префаб мирового представления (worldDropPrefab)

1. Создайте GameObject с моделью оружия
2. Добавьте **Collider** (Is Trigger = false, для рейкаста подбора)
3. Добавьте компонент `WorldWeapon`
4. Установите Layer на тот, что входит в Pickup Mask
5. Сохраните как префаб

### Шаг 5: Привязать всё в WeaponData

1. Откройте созданный WeaponData asset
2. Перетащите префабы в поля `weaponPrefab`, `projectilePrefab`, `worldDropPrefab`

### Шаг 6: Разместить на сцене

1. Перетащите `worldDropPrefab` на сцену
2. В компоненте `WorldWeapon` укажите ссылку на `WeaponData`
3. Запустите — оружие будет вращаться и покачиваться, ожидая подбора

---

## 4. Настройка WeaponData

### Примеры конфигурации

#### Пистолет

| Поле | Значение |
|------|---------|
| weaponName | Pistol |
| weaponID | pistol_01 |
| weaponType | Pistol |
| firingMode | SemiAuto |
| damageType | Projectile |
| damage | 15 |
| fireRate | 4 |
| maxAmmo | 12 |
| reloadTime | 1.5 |
| infiniteAmmo | false |
| projectileSpeed | 40 |
| projectileLifetime | 2 |
| spreadAngle | 2 |
| pelletsPerShot | 1 |

#### Автомат

| Поле | Значение |
|------|---------|
| weaponName | Assault Rifle |
| weaponID | rifle_01 |
| weaponType | Rifle |
| firingMode | Automatic |
| damageType | Projectile |
| damage | 10 |
| fireRate | 10 |
| maxAmmo | 30 |
| reloadTime | 2 |
| projectileSpeed | 50 |
| projectileLifetime | 3 |
| spreadAngle | 5 |
| pelletsPerShot | 1 |

#### Ближний бой (меч)

| Поле | Значение |
|------|---------|
| weaponName | Sword |
| weaponID | melee_sword_01 |
| weaponType | Melee |
| firingMode | Melee |
| damageType | Melee |
| damage | 30 |
| fireRate | 2 |
| maxAmmo | 1 |
| infiniteAmmo | true |
| meleeRange | 2.5 |
| meleeAngle | 120 |

#### Лук

| Поле | Значение |
|------|---------|
| weaponName | Bow |
| weaponID | bow_01 |
| weaponType | Bow |
| firingMode | Charged |
| damageType | Projectile |
| damage | 20 |
| fireRate | 1 |
| maxAmmo | 15 |
| reloadTime | 1 |
| chargeTime | 1.5 |
| chargeDamageMultiplier | 2.5 |
| projectileSpeed | 35 |
| projectileLifetime | 4 |

#### Дробовик

| Поле | Значение |
|------|---------|
| weaponName | Shotgun |
| weaponID | shotgun_01 |
| weaponType | Shotgun |
| firingMode | SemiAuto |
| damageType | Projectile |
| damage | 8 |
| fireRate | 1.5 |
| maxAmmo | 6 |
| reloadTime | 2.5 |
| projectileSpeed | 30 |
| projectileLifetime | 1 |
| spreadAngle | 15 |
| pelletsPerShot | 8 |

#### Лазер

| Поле | Значение |
|------|---------|
| weaponName | Laser Gun |
| weaponID | laser_01 |
| weaponType | Laser |
| firingMode | Beam |
| damageType | Beam |
| damage | 5 |
| fireRate | 1 |
| maxAmmo | 100 |
| infiniteAmmo | false |
| beamRange | 50 |
| beamDamagePerSecond | 15 |

---

## 5. Настройка префабов

### Префаб оружия (weaponPrefab)

Иерархия:
```
Pistol_Weapon (WeaponView)
├── Model              (3D-модель)
├── MuzzlePoint        (пустой Transform на конце ствола)
├── MuzzleFlash        (ParticleSystem)
└── [BeamRenderer]     (LineRenderer — только для лучевого оружия)
```

**WeaponView** — компонент на корневом объекте.

Настройки WeaponView в Inspector:
- `Muzzle Point` → перетащить Transform `MuzzlePoint`
- `Melee Hit Point` → перетащить точку удара (для melee-оружия)
- `Beam Renderer` → LineRenderer (для лазера)
- `Muzzle Flash` → ParticleSystem (опционально)

### Префаб снаряда (projectilePrefab)

Иерархия:
```
Bullet (Projectile, Rigidbody, SphereCollider)
├── Model (маленькая сфера или модель пули)
└── Trail (TrailRenderer — опционально)
```

Настройки:
- **Rigidbody**: Use Gravity = false, Is Kinematic = false
- **Collider**: Is Trigger = true
- **Projectile**: без настроек в Inspector (инициализируется через код)

### Префаб мирового оружия (worldDropPrefab)

Иерархия:
```
Pistol_WorldDrop (WorldWeapon, BoxCollider)
├── Model (3D-модель оружия)
└── Glow (опциональный эффект свечения)
```

Настройки:
- **Collider**: Is Trigger = false (нужен для рейкаста!)
- **Layer**: специальный слой (например, "Pickup"), включённый в Pickup Mask
- **WorldWeapon**: `Weapon Data` → перетащить ScriptableObject

---

## 6. Подключение системы в Unity

### Шаг 1: Создать Layer для подбора

1. **Edit → Project Settings → Tags and Layers**
2. Добавьте слой `Pickup` (например, Layer 8)

### Шаг 2: Создать Layer для попадания

1. Добавьте слой `Enemy` или `Hittable` для объектов, по которым можно стрелять

### Шаг 3: Настроить префаб персонажа

1. Откройте префаб персонажа
2. Убедитесь, что на нём есть компоненты:
   - `CharacterView` — в поле `Weapon Pivot` перетащить Transform (см. раздел 7)
   - `WeaponSlot` — в поле `Hit Mask` выберите слои врагов/целей
3. Компоненты `WeaponPickupSystem` добавляются автоматически в `GameplayEntryPoint`

### Шаг 4: Настроить GameplayEntryPoint

На объекте `GameplayEntryPoint` в сцене:
- `Character Prefab` → префаб персонажа
- `Input System Prefab` → префаб с UnityInputSystem
- `Player Camera` → камера игрока (если null, используется Camera.main)
- `Scene UI Root Prefab` → как обычно

### Шаг 5: Регенерировать Input System

1. Откройте `Assets/__GAME__/Source/Game/Gameplay/Player/InputSystem.inputactions`
2. Unity должен автоматически регенерировать C#-класс
3. Если нет — нажмите ПКМ → Reimport
4. Убедитесь, что в сгенерированном классе есть actions: `Interact`, `Reload`, `SwitchWeapon`

### Шаг 6: Разместить оружие на сцене

1. Перетащите `worldDropPrefab` на сцену
2. Настройте `WorldWeapon.Weapon Data`
3. Установите Layer объекта = `Pickup`

### Шаг 7: Настроить ProjectilePool (опционально)

1. Создайте пустой GameObject на сцене
2. Добавьте компонент `ProjectilePool`
3. Настройте `Initial Pool Size` (рекомендуется 20–50)

---

## 7. Настройка WeaponPivot

### Что это

**WeaponPivot** — это Transform-точка на модели персонажа, к которой прикрепляется оружие при экипировке. Все инстанциированные prefab'ы оружия становятся дочерними объектами этого Transform.

### Почему WeaponPivot, а не другие подходы

| Подход | Плюсы | Минусы |
|--------|-------|--------|
| **WeaponPivot (Transform)** | Просто, быстро, стандарт индустрии | Нет IK-привязки к рукам |
| Socket System | Множественные точки крепления | Сложнее, нужна для AAA |
| Bone Attachment | Анимация управляет позицией | Зависимость от скелета |

Для проекта в стиле Soul Knight **WeaponPivot** — оптимальный выбор.

### Как настроить

1. Откройте префаб персонажа
2. Создайте пустой дочерний GameObject и назовите его `WeaponPivot`
3. Разместите его примерно в зоне руки:
   - **Позиция**: `(0.5, 1.0, 0.5)` для стандартного персонажа (корректируйте под модель)
   - **Ротация**: направьте вперёд (ось Z)
4. В компоненте `CharacterView` перетащите `WeaponPivot` в поле `Weapon Pivot`

### Важно

- Оружие при экипировке устанавливается с `localPosition = (0,0,0)` и `localRotation = identity` относительно WeaponPivot
- Поэтому ориентация WeaponPivot определяет направление оружия
- Если модель оружия в префабе смещена, корректируйте сам префаб, а не WeaponPivot

---

## 8. Тестирование системы на сцене

### Минимальная тестовая сцена

1. **Создайте сцену** с:
   - Плоскость (пол)
   - Камера (от первого/третьего лица)
   - Directional Light

2. **Разместите персонажа**:
   - Инстанциируется через GameplayEntryPoint, но для теста можно руками

3. **Создайте тестовые ScriptableObject**:
   - `Create → Game → Weapon` → `Pistol_Data` (SemiAuto/Projectile)
   - `Create → Game → Weapon` → `Rifle_Data` (Automatic/Projectile)
   - `Create → Game → Weapon` → `Sword_Data` (Melee)

4. **Создайте тестовые префабы**:
   - Для каждого оружия: weaponPrefab, projectilePrefab (если нужно), worldDropPrefab

5. **Разместите WorldWeapon на сцене**:
   - По 1 экземпляру каждого оружия
   - Layer = Pickup

6. **Создайте мишень**:
   - Куб с компонентом, реализующим `IDamageable`:
   ```csharp
   public class TestTarget : MonoBehaviour, IDamageable
   {
       public void TakeDamage(int damage)
       {
           Debug.Log($"Получен урон: {damage}");
       }
   }
   ```
   - Layer = Enemy/Hittable

### Порядок тестирования

1. **Запустите** игру
2. **Подойдите** к оружию, наведите центр экрана
3. **Нажмите E** — оружие должно появиться в руках
4. **ЛКМ** — атака (зависит от типа)
5. **R** — перезарядка
6. **Подберите второе оружие** (другого типа)
7. **Q** — переключение между оружиями
8. **Подберите третье** — активное должно выброситься в мир
9. Стреляйте по мишени — в консоли должен появиться лог урона

---

## 9. Отладка типичных проблем

### Оружие не подбирается

| Проблема | Решение |
|----------|---------|
| Рейкаст не попадает | Проверьте Layer объекта WorldWeapon и Pickup Mask на WeaponPickupSystem |
| WorldWeapon не настроен | Убедитесь, что поле `Weapon Data` заполнено |
| Collider отсутствует | WorldWeapon должен иметь Collider (NOT trigger) |
| Камера null | Убедитесь, что `Player Camera` назначена в GameplayEntryPoint или есть Camera.main |
| Расстояние | `Pickup Range` по умолчанию = 3м. Увеличьте если нужно |

### Оружие не стреляет

| Проблема | Решение |
|----------|---------|
| CanAttack = false | Проверьте патроны и статус перезарядки в WeaponModel |
| Неправильный FiringMode | Убедитесь, что FiringMode в WeaponData соответствует типу оружия |
| Muzzle Point = null | На префабе оружия в WeaponView должен быть назначен Muzzle Point |
| Projectile Prefab = null | Для Projectile DamageType обязателен projectilePrefab |
| Нет компонента Projectile | На префабе снаряда должен быть компонент Projectile |

### Снаряды не наносят урон

| Проблема | Решение |
|----------|---------|
| Нет IDamageable | На цели должен быть компонент, реализующий IDamageable |
| Collider trigger | Снаряд должен иметь Collider с Is Trigger = true; цель — обычный Collider |
| Неправильный Hit Mask | В WeaponSlot проверьте, что Hit Mask включает слой цели |

### Оружие не переключается

| Проблема | Решение |
|----------|---------|
| Только 1 оружие | Переключение работает только если оба слота заняты |
| Input не работает | Регенерируйте InputSystem.inputactions (Reimport) |

### Оружие неправильно расположено

| Проблема | Решение |
|----------|---------|
| Смещение | Корректируйте позицию модели внутри weaponPrefab, НЕ WeaponPivot |
| Ротация | Ось Z оружия должна смотреть вперёд в локальных координатах |

---

## 10. Рекомендации по улучшению

### Система отдачи (Recoil)

Добавление визуальной и камерной отдачи значительно улучшает ощущение от стрельбы:
- **Камерная отдача**: смещение камеры вверх на случайный угол при каждом выстреле, возврат через Lerp
- **Визуальная отдача**: смещение weaponPrefab назад с пружинным возвратом (через PrimeTween, который уже есть в проекте)
- Реализация: создать `IRecoilHandler` и применять через WeaponPresenter после каждого выстрела
- Параметры отдачи можно добавить в WeaponData

### Анимации переключения оружия

Плавные анимации экипировки/снятия:
- fade-in/fade-out через масштаб или прозрачность
- анимация "достать из-за спины" / "убрать за спину"
- задержка перед возможностью стрелять после переключения
- Реализация: корутина или PrimeTween в WeaponView.Equip/Unequip

### Пулинг снарядов (ProjectilePool)

Базовый пулинг уже реализован. Улучшения:
- Привязка пула к конкретному типу снаряда (по InstanceID префаба)
- Prewarming при загрузке сцены
- Автоматическое расширение пула при нехватке

### Визуальные эффекты оружия

- **Trail для снарядов**: TrailRenderer на projectilePrefab
- **Гильзы**: спавн гильз из порта выброса (ещё один Transform на WeaponView)
- **Декали попадания**: Instantiate декали на месте попадания рейкаста
- **Shake камеры**: при стрельбе крупнокалиберным оружием

### UI-индикаторы оружия

Интеграция с MVVM-слоем для UI:
- Иконка активного оружия (поле `weaponIcon` в WeaponData уже предусмотрено)
- Счётчик патронов (текущие / максимальные)
- Индикатор перезарядки (прогресс-бар)
- Пикап-подсказка ("Press E to pick up [WeaponName]") — когда WeaponPickupSystem.CurrentTarget != null
- Слоты инвентаря (2 иконки, подсветка активного)
- Реализация: создать WeaponUIViewModel, подписаться на WeaponInventory.OnWeaponChanged

### Система модификаторов

Для расширения как в Soul Knight:
- Бонусы к урону, скорости стрельбы, размеру снарядов
- Специальные эффекты (отравление, заморозка, поджог)
- Реализация: композиция IWeaponModifier, применяемый через WeaponPresenter

### Кастомные поведения стрельбы

Для уникальных типов оружия:
1. Создать класс, реализующий `IFiringBehavior`
2. Добавить новое значение в enum `FiringMode`
3. Зарегистрировать в `FiringBehaviorFactory`
4. Теперь дизайнер может выбрать это поведение в WeaponData

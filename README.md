# Tower Elemental Defense Game

![Unity](https://img.shields.io/badge/Unity-2022.3%2B-black?style=for-the-badge&logo=unity)
![C#](https://img.shields.io/badge/C%23-Programming-blue?style=for-the-badge&logo=c-sharp)

**Tower Elemental Defense Game** là một dự án game chiến thuật phòng thủ tháp (Tower Defense) kết hợp với các cơ chế Auto-Chess (TFT - Teamfight Tactics). Trong game, người chơi sẽ sử dụng vàng để mua các tháp phòng thủ nguyên tố, quản lý hàng chờ (Bench), nâng cấp và lai tạo các nguyên tố với nhau để tạo ra các loại tháp mới mạnh mẽ hơn nhằm chống lại các đợt tấn công (Wave) của kẻ thù.

---

## 🌟 Tính Năng Nổi Bật

### 1. Cơ chế Chuẩn Bị & Chiến Đấu (TFT Style)
Lối chơi chia làm 2 giai đoạn chính giống cơ chế Auto-Chess:
- **Phase Chuẩn bị (Pause):** Thời gian giữa các Wave. Người chơi sử dụng vàng để mua tháp từ Shop, sắp xếp đội hình từ Hàng chờ (Bench) lên Sân đấu (Grid), thực hiện gộp tháp và nâng cấp.
- **Phase Chiến đấu (Resume):** Kẻ thù xuất hiện theo từng đợt. Tháp trên sân tự động nhắm bắn và tấn công kẻ thù để bảo vệ Nhà Chính (Nexus/Base).

### 2. Hệ Thống Nguyên Tố & Lai Tạo (Elemental & Merge System)
Người chơi có thể gộp 2 tháp lại với nhau (Kéo thả tháp này chồng lên tháp kia) để tạo ra tháp mạnh hơn:
- **Nâng cấp (Upgrade):** Ghép 2 tháp **cùng hệ và cùng cấp** để tăng lên 1 cấp (Tối đa Cấp 3).
- **Lai tạo (Hybrid):** Ghép 2 tháp **khác hệ** để tạo ra một nguyên tố đột biến hoàn toàn mới:
  - 🔥 **Lửa (Fire)** + 💧 **Nước (Water)** = 💥 **Bom Nhiệt (Thermobaric)**
  - 🔥 **Lửa (Fire)** + 🪨 **Đất (Earth)** = 🌋 **Dung Nham (Magma/Lava)**
  - 🪨 **Đất (Earth)** + 💧 **Nước (Water)** = 🌿 **Đầm Lầy (Swamp)**

### 3. Quản Lý Kinh Tế & Phần Thưởng
- **Cửa hàng (Shop):** Mua ngẫu nhiên các tháp nguyên tố cơ bản.
- **Vàng (Gold):** Kiếm được bằng cách tiêu diệt quái vật hoặc vượt qua các Wave.
- **Phần thưởng Wave (Wave Rewards):** Nhận được các lựa chọn nâng cấp/phần thưởng sau khi phòng thủ thành công một đợt lính.

---

## 📁 Cấu Trúc Dự Án (Thư Mục Scripts)

Mã nguồn (C#) được tổ chức theo các Module riêng biệt để dễ dàng quản lý và mở rộng:

- `Managers/`: Trái tim của game, chứa các hệ thống quản lý tổng thể.
  - `GameManager`: Quản lý Game State (Pause, Resume, Freeze, Victory, GameOver).
  - `GameEconomy` & `ShopManager`: Xử lý tiền tệ (Vàng) và hệ thống cửa hàng.
  - `WaveSpawner` & `BaseWaveData`: Quản lý kịch bản xuất hiện của kẻ địch theo từng đợt.
  - `BenchManager` & `GridManager`: Quản lý lưới ô vuông trên sân đấu và hàng chờ.
- `Towers/`: Chứa các logic liên quan đến Tháp phòng thủ.
  - `TowerController`, `TowerAttack`, `TowerHealth`: Hành vi cơ bản của tháp (Bắn, Máu, Trạng thái hoạt động).
  - `TowerDrag` & `TowerPlacementManager`: Hệ thống kéo thả tháp từ Hàng chờ lên Sân đấu.
  - `MergeManager`: Chịu trách nhiệm xử lý logic gộp tháp (Nâng cấp & Lai tạo).
- `Enemies/`: Trí tuệ nhân tạo (AI) và hành vi của kẻ địch.
  - `EnemyMovement`, `EnemyHealth`, `EnemyAnimation`: Di chuyển theo đường đi (Pathfinding), trừ máu và hiệu ứng.
- `Base/`:
  - `BaseHealth`: Quản lý sinh lực của Nhà Chính. Game Over khi máu Base bằng 0.
- `UI/`:
  - Quản lý các giao diện người dùng như Thanh máu (`HealthBar`), Shop (`ShopCardUI`), Màn hình Thắng/Thua (`GameOverUI`), Khu vực bán tháp (`SellZone`).
- `Weapons/`:
  - `Bullet`: Logic đường đạn, sát thương va chạm (Collision).

---

## 🎮 Hướng Dẫn Chơi Cơ Bản
1. **Bắt đầu game:** Bạn sẽ được cấp một lượng vàng khởi điểm (100 vàng).
2. **Mua tháp:** Mở Shop và mua các tháp nguyên tố. Tháp mới mua sẽ xuất hiện ở **Hàng Chờ (Bench)**.
3. **Triển khai:** Kéo thả tháp từ Hàng chờ lên các ô trống trên **Sân Đấu (Grid)**.
4. **Nâng cấp/Lai tạo:** Kéo một tháp đè lên một tháp khác (thỏa mãn công thức ghép) để tạo ra tháp mạnh hơn.
5. **Bắt đầu đợt tấn công:** Nhấn nút `Start Wave` để bắt đầu phòng thủ.
6. **Bảo vệ Nhà Chính:** Nếu quái vật vượt qua hàng phòng ngự và phá hủy Base, bạn sẽ thua (Game Over). Sống sót qua tất cả các Wave để giành chiến thắng (Victory)!

---

## 🛠️ Cài Đặt & Mở Dự Án
1. Đảm bảo bạn đã cài đặt **Unity Editor** (Khuyến nghị phiên bản 2022.3 LTS hoặc mới hơn).
2. Clone repository này hoặc tải file ZIP về máy giải nén.
3. Mở **Unity Hub**, chọn `Add project from disk` và trỏ tới thư mục `Tower Elemental Defense Game`.
4. Mở Scene chính tại `Assets/Scenes/Gameplay` (hoặc `MainMenu`).
5. Bấm nút **Play** ở giữa màn hình Editor để trải nghiệm.

---

**Tác giả / Developer:** Dự án cá nhân / Nhóm phát triển (Thay đổi tên tại đây).
Chúc bạn có những trải nghiệm thú vị với Tower Elemental Defense Game!

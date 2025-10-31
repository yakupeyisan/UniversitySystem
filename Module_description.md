# 🎓 ORION ÜNİVERSİTE YÖNETİM SİSTEMİ
## 24 MODÜLÜN DETAYLI AÇIKLAMASI

**Tarih:** 31 Ekim 2025  
**Versiyon:** 1.0

---

## 1️⃣ WALLET SYSTEM (Cüzdan & Dijital Kart Sistemi)

### 🎯 Amaç
Üniversite içinde tüm ödemeleri elektronik ortamda gerçekleştiren, QR kod ve barkod tabanlı dijital kart sistemi.

### 📋 Ana Sorumluluklar
- Dijital kimlik kartı (AccessCard) yönetimi
- Kişi başına bir cüzdan (Wallet) hesabı
- Bakiye yönetimi ve işlem geçmişi
- Kart engelleme/kayıp işlemleri
- QR kod ve barkod doğrulama

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Dijital Kart** | QR Kod + Barkod + Kart Numarası |
| **Bakiye Takibi** | Gerçek zamanlı bakiye görüntüsü |
| **İşlem Geçmişi** | Tüm işlemlerin detaylı kaydı |
| **Kart Yönetimi** | İssue, Renew, Block, Cancel |
| **Multi-Currency** | TRY ve diğer para birimleri |
| **Refund** | Para iadesi işlemleri |

### 🔗 Bağımlılıkları
- ✅ PersonMgmt (Kullanıcı bilgileri)
- ✅ Core (Base entities, specifications)

### 🔄 Bağlandığı Modüller
→ VirtualPOS (Ödeme processing)  
→ StudentFees (Harç ödemeleri)  
→ Cafeteria (Yemek ödemeleri)  
→ Parking (Park ücreti ödemeleri)  
→ Library (Kütüphane cezası ödemeleri)  
→ AccessControl (Kart okuma)

### 💾 Temel Entity'ler
```
AccessCard:
  - CardNumber (Unique)
  - QRCode (Unique)
  - Barcode (Unique)
  - IssueDate, ExpiryDate
  - CardStatus (Active, Blocked, Lost, Expired)
  - PersonId (FK)
  - BlockReason, BlockedAt, BlockedBy

Wallet:
  - PersonId (Unique FK)
  - Balance (Decimal)
  - Currency (TRY)
  - LastTransactionDate
  - IsActive

WalletTransaction:
  - WalletId (FK)
  - TransactionType (Load, Purchase, Refund, Transfer)
  - Amount
  - BalanceBefore, BalanceAfter
  - MerchantName
  - PaymentMethod
  - ReferenceNumber
  - Status
```

### 📡 Temel API Endpoints
```
POST   /api/v1/wallet/cards/issue              - Kart düzenle
POST   /api/v1/wallet/cards/block              - Kartı engelle
POST   /api/v1/wallet/load                     - Cüzdan yükle
GET    /api/v1/wallet/balance/{personId}      - Bakiye sorgula
GET    /api/v1/wallet/transactions/{personId} - İşlem geçmişi
POST   /api/v1/wallet/refund                   - İade işlemi
```

---

## 2️⃣ VIRTUAL POS (Ödeme Gateway'i Entegrasyonu)

### 🎯 Amaç
Türkiye'deki yaygın ödeme sistemleri (Get724, NestPay, Iyzico, PayTR) ile entegre ödeme işlemi gerçekleştirme.

### 📋 Ana Sorumlulukları
- Kredi kartı, banka kartı, havale/EFT ödemeleri
- Her gateway'e özel adapter pattern
- Ödeme doğrulama ve onay
- İade işlemleri
- Webhook ve callback handling
- Hata yönetimi ve retry logic

### ✨ Temel Özellikler
| Gateway | Durum | Notlar |
|---------|-------|--------|
| **Get724** | 🟡 Kurumsal | Gümrük Müdürlüğü entegrasyonu |
| **NestPay** | 🟡 Kurumsal | Banka POS sistemleri |
| **Iyzico** | 🟢 Modern | Taksitlendirme desteği |
| **PayTR** | 🟢 Modern | API-first approach |

### 🔄 Ödeme Akışı
```
1. Müşteri ödeme bilgisi gönder
   ↓
2. VirtualPOS → Gateway'e ilet
   ↓
3. Gateway → Banka process
   ↓
4. Banka → Başarı/Başarısızlık yanıtı
   ↓
5. VirtualPOS → Webhook alıyor
   ↓
6. Payment DB → Kayıt güncellenir
   ↓
7. Wallet/StudentFees → İşlem tamamlanır
```

### 🔗 Bağımlılıkları
- ✅ Wallet (Cüzdan sistemi)
- 🔄 External Services (Payment Gateways)

### 🔄 Bağlandığı Modüller
← Wallet (Cüzdan yüklemesi)  
← StudentFees (Harç ödemeleri)  
← Cafeteria (Yemek ödemeleri)  

### 💾 Temel Entity'ler
```
Payment:
  - PaymentId (PK)
  - PersonId (FK)
  - Amount
  - Currency (TRY)
  - PaymentMethod (CreditCard, Havale, etc.)
  - PaymentGateway (Get724, NestPay, Iyzico, PayTR)
  - Status (Pending, Completed, Failed, Refunded)
  - TransactionReference
  - GatewayTransactionId
  - WebhookData

Refund:
  - RefundId (PK)
  - PaymentId (FK)
  - RefundAmount
  - RefundDate
  - RefundReason
  - Status
```

### 📡 Temel API Endpoints
```
POST   /api/v1/payments/initiate              - Ödeme başlat
POST   /api/v1/payments/callback/{gateway}   - Webhook endpoint
POST   /api/v1/payments/verify/{paymentId}   - Ödeme doğru mu?
GET    /api/v1/payments/{paymentId}          - Ödeme detayı
POST   /api/v1/payments/refund               - İade işlemi
```

---

## 3️⃣ STUDENT FEES (Öğrenci Harçları & Ücretlendirme)

### 🎯 Amaç
Öğrenci harçlarını tanımlama, faturalandırma, ödeme planı oluşturma ve borç takibi.

### 📋 Ana Sorumlulukları
- Harç türleri tanımlama
- Dönem bazlı ücretlendirme
- Öğrenci başına fatura oluşturma
- Ödeme planı ve taksitlendirme
- Ödeme takibi
- Borç öğrenci raporları

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Harç Türleri** | Tuition, Per-Course, Summer, Lab, Document |
| **Fatura** | Otomatik fatura oluşturma |
| **Ödeme Planı** | Taksit dönemleri (1, 2, 3, 6, 12 ay) |
| **Borç Takibi** | Vadesi geçmiş öğrenci listesi |
| **İndirim** | Burs, özel indirim uygulaması |
| **Kayıt Dondurma** | Borçlu öğrenciler kayıt yapamaz |

### 🔗 Bağımlılıkları
- ✅ Academic (Program bilgileri, dönem)
- ✅ PersonMgmt (Öğrenci bilgileri)
- 🔄 VirtualPOS (Ödeme processing)
- 🔄 Wallet (Ödeme kaynağı)
- 🔄 Finance (Mali raporlama)

### 💾 Temel Entity'ler
```
StudentFee:
  - StudentFeeId (PK)
  - StudentId (FK)
  - SemesterId (FK)
  - FeeType (enum)
  - Amount
  - DueDate
  - PaidAmount
  - RemainingAmount
  - Status (Pending, PartiallyPaid, FullyPaid, Overdue)

Invoice:
  - InvoiceId (PK)
  - StudentFeeId (FK)
  - InvoiceNumber (Unique)
  - IssuedDate
  - DueDate
  - Total
  - Status

PaymentPlan:
  - PaymentPlanId (PK)
  - StudentFeeId (FK)
  - InstallmentCount
  - InstallmentAmount
  - StartDate
  - Installments[] (Child records)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/student-fees/define           - Harç tipi tanımla
POST   /api/v1/student-fees/calculate        - Öğrenciye harç hesapla
GET    /api/v1/student-fees/{studentId}     - Öğrenci harç bilgisi
POST   /api/v1/invoices/generate             - Fatura oluştur
POST   /api/v1/payment-plans/create          - Ödeme planı oluştur
GET    /api/v1/student-fees/delinquent       - Borçlu öğrenciler
```

---

## 4️⃣ LEAVE MANAGEMENT (İzin Yönetimi)

### 🎯 Amaç
Personel izin talep, onay ve bakiye yönetimi sistemi.

### 📋 Ana Sorumlulukları
- İzin türleri tanımlama (Yıllık, Mazeret, Raporlu, Ücretsiz)
- İzin bakiyesi hesaplama
- İzin talep ve onay workflow'u
- Mazeret belge yönetimi
- Toplu izin planlama

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **İzin Türleri** | Yıllık, Mazeret, Raporlu, Ücretsiz, Koma, Evlilik |
| **Bakiye Hesaplama** | Yıl bazlı, devreden günler |
| **Onay Sistemi** | Amirinden onay gerekli |
| **Mazeret Belge** | Sağlık raporu, ölüm cüzdanı vb. |
| **Çakışma Kontrol** | Aynı dönemde birden fazla izin prevent |
| **Raporlama** | İzin kullanım istatistikleri |

### 🔗 Bağımlılıkları
- ✅ PersonMgmt (Çalışan bilgileri)
- 🔄 HR (Çalışan yönetimi)

### 🔄 Bağlandığı Modüller
← HR (Çalışan masraf hesapla)  
← Payroll (Bonus/indirim hesabı)  

### 💾 Temel Entity'ler
```
LeaveType:
  - LeaveTypeId (PK)
  - LeaveTypeName (Yıllık, Mazeret, etc.)
  - MaxDaysPerYear
  - RequiresApproval (bool)
  - IsPaid (bool)
  - IsActive

LeaveBalance:
  - LeaveBalanceId (PK)
  - EmployeeId (FK)
  - LeaveTypeId (FK)
  - Year
  - TotalDays
  - UsedDays
  - RemainingDays
  - CarryForwardDays

LeaveRequest:
  - LeaveRequestId (PK)
  - EmployeeId (FK)
  - LeaveTypeId (FK)
  - StartDate
  - EndDate
  - TotalDays
  - Reason
  - DocumentPath (nullable)
  - Status (Pending, Approved, Rejected, Cancelled)
  - ApprovedBy
  - ApprovalDate
```

### 📡 Temel API Endpoints
```
POST   /api/v1/leave/request                 - İzin talep et
GET    /api/v1/leave/balance/{employeeId}   - İzin bakiyesi
GET    /api/v1/leave/requests/{employeeId}  - Talep geçmişi
POST   /api/v1/leave/approve                 - İzni onayla
POST   /api/v1/leave/reject                  - İzni reddet
GET    /api/v1/leave/report                  - İzin kullanım raporu
```

---

## 5️⃣ HR (İnsan Kaynakları Yönetimi)

### 🎯 Amaç
Personel kayıt, sözleşme, pozisyon ve organizasyon yapısı yönetimi.

### 📋 Ana Sorumlulukları
- Çalışan kayıt sistemi
- Sözleşme yönetimi
- Pozisyon ve kadro tanımlama
- Vardiya yönetimi
- Raporlama ilişkileri (Hierarchical)
- Performans izleme altyapısı

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Çalışan Türleri** | Academic, Administrative, Contractor, Temporary |
| **Sözleşme** | Tarihler, şartlar, imzalar |
| **Pozisyon Hiyerarşisi** | Manager → Reports hierarchy |
| **Vardiya** | Sabah, Öğle, Gece, Esnek |
| **SGK/Vergi Alanları** | SGK NO, Vergi NO, Sosyal Yardım |
| **Raporlama** | İK analitiği, kadro grafikleri |

### 🔗 Bağımlılıkları
- ✅ PersonMgmt (Kişi bilgileri)
- 🔄 Leave (İzin sistemi)
- 🔄 Payroll (Bordro temeliyolumu)

### 🔄 Bağlandığı Modüller
← Finance (Maliyet merkezi)  
← Payroll (Bordro hesaplama)  
← Leave (İzin yönetimi)  
← Performance (Değerlendirme)  

### 💾 Temel Entity'ler
```
Employee:
  - EmployeeId (PK)
  - PersonId (FK)
  - EmployeeNumber (Unique)
  - DepartmentId (FK)
  - PositionTitle
  - EmploymentType (FullTime, PartTime, Contract, Temporary)
  - HireDate
  - TerminationDate (nullable)
  - ReportingTo (Recursive FK)
  - BaseSalary

Contract:
  - ContractId (PK)
  - EmployeeId (FK)
  - ContractType (Permanent, Temporary, Seasonal)
  - StartDate
  - EndDate
  - Salary
  - Benefits
  - DocumentPath
  - Status

Position:
  - PositionId (PK)
  - PositionTitle
  - DepartmentId (FK)
  - Level (0-10, hierarchical)
  - BaseSalary
  - MaxSalary
  - Responsibilities
  - RequiredQualifications
```

### 📡 Temel API Endpoints
```
POST   /api/v1/employees/register            - Çalışan kaydet
POST   /api/v1/contracts/create              - Sözleşme yarat
GET    /api/v1/employees/{employeeId}       - Çalışan bilgisi
GET    /api/v1/employees/department/{deptId} - Birimde çalışanlar
POST   /api/v1/positions/define              - Pozisyon tanımla
GET    /api/v1/hr/organization-chart         - Organizasyon şeması
```

---

## 6️⃣ PAYROLL (Bordro Sistemi) 🇹🇷 TÜRKİYE UYUMLU

### 🎯 Amaç
Türk mühasebe mevzuatına uygun personel bordro hesaplama, SGK ve vergi raporlaması.

### 📋 Ana Sorumlulukları
- Maaş yapısı tanımlama
- **SGK primleri hesaplama** (20.5% işçi, 22.5% işveren)
- **Gelir vergisi hesaplama** (15%-40% aşamalı)
- Muhtasar beyanname oluşturma
- SGK SOCRATES entegrasyonu
- GİB e-Declaration raporu
- 5-yıl audit trail tutma

### ✨ Temel Özellikler (🇹🇷 TÜRK MEVZUAT)
| Özellik | Hesaplama |
|---------|-----------|
| **SGK Çalışan** | Brüt Maaş × %20.5 |
| **SGK İşveren** | Brüt Maaş × %22.5 (+ %8 İşsizlik) |
| **Gelir Vergisi** | Aşamalı (15%, 20%, 27%, 40%) |
| **Muhtasar** | Aylık beyanname (son 25. günü) |
| **SGK SOCRATES** | Entegre raporlama |
| **GİB e-Declaration** | Yıllık vergi raporu |
| **Audit Trail** | 5-yıl kayıt (Hukuki gereklilik) |

### ⚠️ KRİTİK
- Hatalı hesaplama = Cezalar + Faiz
- MUTLAKA muhasebeci danışmanı ile
- Tüm mali yıllar için retrospektif düzeltme

### 🔗 Bağımlılıkları
- 🔄 HR (Çalışan bilgileri)
- 🔄 Leave (İzin kesintileri)
- 🔄 Finance (Mali raporlama)

### 🔄 Bağlandığı Modüller
→ Finance (Muhasebe kaydı)  
→ Bank (Ödeme dosyası)  

### 💾 Temel Entity'ler
```
SalaryStructure:
  - SalaryStructureId (PK)
  - EmployeeId (FK)
  - BaseSalary
  - HousingAllowance
  - TransportationAllowance
  - MealAllowance
  - OtherAllowances
  - AcademicIncentive
  - EffectiveDate

PayrollRun:
  - PayrollRunId (PK)
  - PayrollMonth (1-12)
  - PayrollYear
  - ProcessedDate
  - ProcessedBy (FK)
  - TotalGrossPay
  - TotalDeductions
  - TotalNetPay
  - Status (Draft, Processed, Approved, Paid)

PayrollDetail:
  - PayrollDetailId (PK)
  - PayrollRunId (FK)
  - EmployeeId (FK)
  - GrossSalary
  - Allowances
  - OvertimePay
  - Bonus
  - TotalGross
  - IncomeTax (Aşamalı hesaplama)
  - SGKEmployee (20.5%)
  - SGKEmployer (22.5%)
  - UnemploymentInsurance (1%)
  - UnionDues (nullable)
  - OtherDeductions
  - TotalDeductions
  - NetPay
  - PaymentDate
  - PaymentMethod (BankTransfer, Cash, Check)
  - SGKStatus (Raporlandı mı?)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/payroll/run                    - Bordro ayı başlat
POST   /api/v1/payroll/calculate              - Bordro hesapla
POST   /api/v1/payroll/approve                - Bordroyu onayla
POST   /api/v1/payroll/pay                    - Ödeme yap (Bank file)
GET    /api/v1/payroll/report/{month}        - Bordro raporu
POST   /api/v1/payroll/sgk-socrates/export   - SGK bildirge
POST   /api/v1/payroll/gib-declaration       - GİB e-Declaration
```

---

## 7️⃣ FINANCE (Mali İşler & Muhasebe)

### 🎯 Amaç
Üniversitenin tüm mali işlemleri (muhasebe, bütçe, giderleri, vergi raporlama) yönetimi.

### 📋 Ana Sorumlulukları
- Genel muhasebe (Defteri kebir)
- Bütçe yönetimi ve takibi
- Gider talep ve onay
- Banka hesap yönetimi
- Bilanço ve gelir-gider tablosu
- GİB e-Declaration entegrasyonu
- Döner sermaye muhasebesi (Üniversiteye özel)

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Genel Muhasebe** | Yevmiye, Defteri Kebir, Mizan, Bilanço |
| **Bütçe** | Yıllık plan, departman bazlı dağılım |
| **Gider Onay** | Talep → Onay → Ödeme workflow |
| **Cari Hesaplar** | Tedarikçi, Personel, Öğrenci, Banka |
| **Raporlar** | Mali tablolar, vergi raporları |
| **Döner Sermaye** | Özel gelir (proje, yayın) paylaşımı |

### 🔗 Bağımlılıkları
- ✅ Core (Base entities)
- 🔄 Payroll (Personel giderleri)
- 🔄 VirtualPOS (Öğrenci ödemeleri)
- 🔄 Procurement (Tedarikçi ödüşümleri)

### 🔄 Bağlandığı Modüller
← Payroll (Bordro gideri)  
← Procurement (Tedarikçi ödemeleri)  
← StudentFees (Öğrenci gelirleri)  
→ Reports (Mali raporlama)  

### 💾 Temel Entity'ler
```
BankAccount:
  - BankAccountId (PK)
  - AccountNumber
  - BankName
  - IBAN
  - Currency (TRY)
  - Balance
  - AccountType (Checking, Savings)
  - IsActive

GeneralLedgerAccount:
  - GLAccountId (PK)
  - AccountCode (Türk Muhasebe Planı: 1-9)
  - AccountName
  - AccountType (Asset, Liability, Equity, Revenue, Expense)
  - ParentAccountId (Recursive, Hiyerarşik)
  - IsActive

Transaction:
  - TransactionId (PK)
  - TransactionNumber (Unique)
  - TransactionDate
  - TransactionType (Income, Expense, Transfer)
  - Amount
  - BankAccountId (FK)
  - GLAccountId (FK)
  - Description
  - ReferenceNumber
  - Status (Pending, Completed, Cancelled)

BudgetAllocation:
  - BudgetAllocationId (PK)
  - FiscalYear
  - DepartmentId (FK)
  - Category (Personal, Equipment, Supplies)
  - AllocatedAmount
  - SpentAmount
  - RemainingAmount (Calculated)
  - Status

Expense:
  - ExpenseId (PK)
  - ExpenseNumber (Unique)
  - ExpenseDate
  - DepartmentId (FK)
  - Category
  - Amount
  - Description
  - RequestedBy (FK)
  - ApprovedBy (FK)
  - Status (Pending, Approved, Rejected, Paid)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/finance/transactions           - İşlem kaydet
GET    /api/v1/finance/gl-accounts            - Muhasebe hesapları
POST   /api/v1/finance/budget/allocate        - Bütçe ata
GET    /api/v1/finance/budget/report          - Bütçe kullanımı
POST   /api/v1/finance/expense/request        - Gider talep
GET    /api/v1/finance/financial-statements  - Mali tablolar
POST   /api/v1/finance/gib-declaration        - GİB raporu
```

---

## 8️⃣ ACCESS CONTROL (Güvenlik & Erişim Kontrol)

### 🎯 Amaç
Kampüs giriş-çıkış, bina erişim, ziyaretçi yönetimi ve güvenlik olayları takibi.

### 📋 Ana Sorumlulukları
- Turnike/Kapı kontrol entegrasyonu
- QR kod ve kart okuma doğrulaması
- Zaman bazlı erişim kontrol
- Ziyaretçi kayıt ve takibi
- Güvenlik olayı kaydı
- Kamera sistemi entegrasyonu
- Acil durum tahliye yönetimi

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Giriş Noktaları** | Turnike, Kapı, Gate (Okuma cihazları) |
| **Erişim Kontrol** | QR, Kart, Biyometrik, Yapay Zeka |
| **Zaman Tabanlı** | Mesai saatleri, hafta sonu kısıtlama |
| **Ziyaretçi** | Misafir kartı, escort takibi, süresi |
| **Güvenlik Olayları** | Tanısız giriş, kapı açık kalma |
| **Kameralar** | Canlı izleme, kayıt, AI yüz tanıma |
| **Tahliye** | Acil durum toplanma alanı, roll-call |

### 🔗 Bağımlılıkları
- ✅ Wallet (Kart sistemi)
- ✅ PersonMgmt (Kişi bilgileri)
- 🔄 Identity (Yetkilendirme)

### 🔄 Bağlandığı Modüller
← Wallet (QR/Card validation)  
← HR (Çalışan erişim yetkileri)  

### 💾 Temel Entity'ler
```
AccessPoint:
  - AccessPointId (PK)
  - AccessPointCode (Unique)
  - AccessPointName
  - Location
  - DeviceType (Turnstile, Door, Gate, Reader)
  - IPAddress
  - IsEntry / IsExit (bool)
  - RequiresApproval
  - IsActive

AccessLog:
  - AccessLogId (PK)
  - PersonId (FK)
  - AccessPointId (FK)
  - AccessCardId (FK)
  - AccessType (Entry, Exit)
  - AccessMethod (QRCode, Card, Biometric, Manual)
  - AccessTime
  - IsAuthorized (bool)
  - DenialReason (nullable)
  - Temperature (nullable, COVID kontrol)

Camera:
  - CameraId (PK)
  - CameraCode (Unique)
  - CameraName
  - Location
  - IPAddress
  - StreamURL
  - CameraType (Indoor, Outdoor, PTZ, Dome)
  - IsActive

SecurityIncident:
  - IncidentId (PK)
  - IncidentNumber (Unique)
  - IncidentType (UnauthorizedEntry, FireAlarm, etc.)
  - Location
  - Description
  - Severity (Low, Medium, High, Critical)
  - ReportedBy (FK)
  - AssignedTo (FK)
  - Status (New, Investigating, Resolved)
  - Resolution
```

### 📡 Temel API Endpoints
```
POST   /api/v1/access-control/validate        - Erişim izni kontrol
POST   /api/v1/access-control/log             - Giriş-çıkış kaydet
GET    /api/v1/access-control/logs/{personId} - Geçmiş
POST   /api/v1/access-control/visitor-check-in - Ziyaretçi kaydı
POST   /api/v1/security-incidents/report      - Olay kaydet
GET    /api/v1/access-control/emergency-status - Acil durum bilgisi
```

---

## 9️⃣ PROCUREMENT (Satın Alma & Tedarikçi Yönetimi)

### 🎯 Amaç
Malzeme ve hizmet alımı, tedarikçi yönetimi, ihale süreci ve sözleşme yönetimi.

### 📋 Ana Sorumlulukları
- Satın alma talepleri
- Tedarikçi kayıt ve değerlendirmesi
- İhale yönetimi (Açık, Belli istekliler, Pazarlık, Doğrudan)
- Sipariş ve teslimat takibi
- Sözleşme yönetimi
- Fatura kontrol ve ödemeleri

### ✨ Temel Özellikler
| İhale Türü | Kullanım |
|-----------|----------|
| **Açık İhale** | Değeri yüksek, herkese açık (KDV altı) |
| **Belli İstekliler** | Kayıtlı tedarikçiler arası |
| **Pazarlık Usulü** | Düşük değerli, hızlı ihtiyaçlar |
| **Doğrudan Temin** | Acil, tek tedarikçi |

### 🔗 Bağımlılıkları
- ✅ Finance (Bütçe kontrol)
- 🔄 Core (Base entities)

### 🔄 Bağlandığı Modüller
→ Inventory (Malzeme stoğu)  
→ Finance (Ödeme işlemi)  

### 💾 Temel Entity'ler
```
Supplier:
  - SupplierId (PK)
  - SupplierCode (Unique)
  - CompanyName
  - TaxNumber (KDV NO)
  - ContactPerson
  - Email, Phone
  - Address
  - PaymentTerms
  - Rating (1-5)
  - IsApproved
  - IsBlacklisted

PurchaseRequest:
  - PurchaseRequestId (PK)
  - RequestNumber (Unique)
  - RequestedBy (FK)
  - DepartmentId (FK)
  - RequestDate
  - Items[] (Line items)
  - TotalEstimatedCost
  - Priority (Low, Medium, High, Urgent)
  - Status (Pending, Approved, Ordered)

Tender:
  - TenderId (PK)
  - TenderNumber (Unique)
  - TenderType (Open, Restricted, Negotiation, DirectAcquisition)
  - Title
  - Description
  - EstimatedAmount
  - AnnouncementDate
  - BidDeadline
  - OpeningDate
  - Status (Announced, BiddingOpen, Evaluating, Awarded)

TenderBid:
  - TenderBidId (PK)
  - TenderId (FK)
  - SupplierId (FK)
  - BidAmount
  - BidDate
  - TechnicalScore
  - FinancialScore
  - IsWinner
  - Status (Submitted, Accepted, Rejected)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/procurement/request            - Satın alma talep
POST   /api/v1/suppliers/register             - Tedarikçi kaydet
POST   /api/v1/tenders/announce               - İhale ilan et
POST   /api/v1/tenders/bids                   - Teklif gönder
GET    /api/v1/tenders/evaluate               - İhaleleri değerlendir
POST   /api/v1/purchase-orders/create         - Sipariş oluştur
GET    /api/v1/purchase-orders/tracking       - Sipariş takibi
```

---

## 🔟 INVENTORY (Envanter & Stok Yönetimi)

### 🎯 Amaç
Kampüs bazlı depo, malzeme stok takibi, depo transferleri ve envanter sayımı.

### 📋 Ana Sorumlulukları
- Depo yönetimi (Her kampüs için ayrı)
- Malzeme/ürün tanımlama
- Stok seviyeleri takibi
- Depo işlemleri (Giriş, Çıkış, Transfer)
- Minimum stok uyarıları
- Envanter sayımı ve mutabakat
- Demirbaş takibi ve zimmet

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Depo Merkezi** | Merkezi depo, departman depoları |
| **Malzeme Kategorileri** | Ofis, Temizlik, Lab, Gıda, Bilgisayar, Mobilya |
| **Stok Yönetimi** | FIFO/LIFO, minimum seviyeleri, yeniden sipariş |
| **Depo Transfer** | Depo arası malzeme hareketi |
| **Sayım** | Tam sayım, döngüsel sayım |
| **Demirbaş** | Kıymet takibi, amortisman, zimmet |

### 🔗 Bağımlılıkları
- 🔄 Procurement (Malzeme alışları)
- ✅ Core (Base entities)

### 🔄 Bağlandığı Modüller
← Procurement (Yeni malzeme)  
→ Finance (Stok değeri)  
→ Cafeteria (Gıda malzemeleri)  

### 💾 Temel Entity'ler
```
Warehouse:
  - WarehouseId (PK)
  - WarehouseCode (Unique)
  - WarehouseName
  - Location
  - WarehouseType (Main, Sub, Department)
  - ManagerId (FK)
  - Capacity

Item:
  - ItemId (PK)
  - ItemCode (Unique)
  - ItemName
  - CategoryId (FK)
  - UnitOfMeasure (pcs, kg, lt, m)
  - MinimumStockLevel
  - ReorderPoint
  - UnitPrice

Stock:
  - StockId (PK)
  - ItemId (FK)
  - WarehouseId (FK)
  - Quantity
  - ReservedQuantity
  - AvailableQuantity (Calculated)

StockMovement:
  - MovementId (PK)
  - ItemId (FK)
  - MovementType (In, Out, Transfer, Adjustment)
  - Quantity
  - MovementDate
  - SourceWarehouse / DestinationWarehouse
  - Reason
  - PerformedBy (FK)

Asset:
  - AssetId (PK)
  - AssetNumber (Unique)
  - AssetName
  - PurchaseDate
  - PurchaseCost
  - Location
  - AssignedToPerson / AssignedToDepartment
  - DepreciationRate
  - Status (Active, Retired, Lost)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/inventory/items/register       - Malzeme tanımla
POST   /api/v1/inventory/stock/in              - Stok giriş
POST   /api/v1/inventory/stock/out             - Stok çıkış
POST   /api/v1/inventory/transfers             - Depo transferi
GET    /api/v1/inventory/stock-levels         - Stok seviyeleri
POST   /api/v1/inventory/count                - Envanter sayımı
GET    /api/v1/inventory/assets               - Demirbaş listesi
```

---

## 1️⃣1️⃣ LIBRARY (Kütüphane Yönetim Sistemi)

### 🎯 Amaç
Kitap ve materyaller ödünç verme, rezervasyon, ceza yönetimi ve tez arşivi.

### 📋 Ana Sorumlulukları
- Kitap ve dergi yönetimi
- Ödünç-İade sistemi
- Kitap rezervasyonu
- Gecikmeli iade cezası hesaplama
- E-kitap ve Tez arşivi
- Online katalog (OPAC)
- Okuyucu istatistikleri

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Kitaplar** | ISBN, Yazar, Yayınevi, Çoklu kopya |
| **Ödünç Kuralları** | Öğrenci 7 gün, Öğretim Üyesi 30 gün |
| **Ceza** | Gecikme günü başına (TL/gün) |
| **E-Kaynaklar** | E-kitaplar, Dergiler, Tezler |
| **Rezervasyon** | Ödünç bekleyen kitaplar |
| **Kütüphane İstatistikleri** | Popüler kitaplar, okuyucu sayıları |

### 🔗 Bağımlılıkları
- ✅ PersonMgmt (Okuyucu bilgileri)
- 🔄 Wallet (Ceza ödemeleri)

### 💾 Temel Entity'ler
```
Book:
  - BookId (PK)
  - ISBN (Unique)
  - Title
  - Author
  - Publisher
  - PublicationYear
  - CategoryId (FK)
  - TotalCopies
  - AvailableCopies
  - Location
  - CoverImagePath

Loan:
  - LoanId (PK)
  - BookId (FK)
  - PersonId (FK)
  - LoanDate
  - DueDate
  - ReturnDate (nullable)
  - RenewalCount
  - Status (Active, Returned, Overdue, Lost)

LibraryFine:
  - FineId (PK)
  - PersonId (FK)
  - LoanId (FK)
  - FineType (LateFee, DamageFee, LostBook)
  - Amount
  - FineDate
  - DueDate
  - PaidDate (nullable)
  - Status (Unpaid, PartiallyPaid, FullyPaid, Waived)

Thesis:
  - ThesisId (PK)
  - Title
  - Author
  - DepartmentId (FK)
  - Year
  - AdvisorId (FK)
  - FilePath
  - AccessLevel (Public, Restricted, Confidential)
  - DownloadCount
```

### 📡 Temel API Endpoints
```
POST   /api/v1/library/books/register         - Kitap kaydı
POST   /api/v1/library/loans/checkout         - Ödünç al (QR)
POST   /api/v1/library/loans/return           - İade et (QR)
POST   /api/v1/library/reservations           - Kitap rezerve et
GET    /api/v1/library/catalog/search         - Katalog arama
GET    /api/v1/library/user/loans             - Ödünç kayıtlarım
GET    /api/v1/library/fines                  - Ceza öde (Wallet)
```

---

## 1️⃣2️⃣ CAFETERIA (Yemekhane & Kafeterya Yönetimi)

### 🎯 Amaç
Menü planlama, ürün yönetimi, satış (POS) ve yemek rezervasyonu.

### 📋 Ana Sorumlulukları
- Menü planlama (Günlük, Haftalık, Aylık)
- Ürün ve fiyat yönetimi
- POS satış sistemi (Cüzdan ile ödeme)
- Yemek rezervasyonu
- Stok yönetimi
- Satış raporları ve analizi
- Diyet seçenekleri

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Yemek Türleri** | Kahvaltı, Öğle, Akşam (Sabah, Öğle, Akşam vardiyası) |
| **Menü** | Günlük güncellenebilir menüler |
| **Diyet** | Vejetaryen, Vegan, Glutensiz, Allergiler |
| **POS** | Cüzdan ile hızlı ödeme |
| **Analiz** | En populer yemekler, satış trendi |
| **Kalori** | Her yemek için kalori bilgisi |

### 🔗 Bağımlılıkları
- 🔄 Wallet (Ödeme sistemi)
- 🔄 Inventory (Malzeme stok)

### 💾 Temel Entity'ler
```
Cafeteria:
  - CafeteriaId (PK)
  - CafeteriaCode (Unique)
  - CafeteriaName
  - Location
  - Capacity
  - ManagerId (FK)
  - CafeteriaType (MainCafeteria, Cafe, FastFood)
  - OpeningTime / ClosingTime

Menu:
  - MenuId (PK)
  - CafeteriaId (FK)
  - MenuDate
  - MealType (Breakfast, Lunch, Dinner)
  - IsActive

MenuItem:
  - MenuItemId (PK)
  - MenuId (FK)
  - ItemName
  - Description
  - Calories
  - Price
  - IsVegetarian / IsVegan / IsGlutenFree
  - Allergens

CafeteriaSale:
  - SaleId (PK)
  - CafeteriaId (FK)
  - MenuItemId (FK)
  - PersonId (FK)
  - SaleDate
  - Quantity
  - UnitPrice
  - TotalAmount
  - PaymentMethod (Card, Wallet, Cash)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/cafeteria/menus/plan           - Menü planla
POST   /api/v1/cafeteria/sales                - Yemek satışı (POS)
GET    /api/v1/cafeteria/menu/today           - Bugünün menüsü
POST   /api/v1/cafeteria/reservations         - Yemek rezerve et
GET    /api/v1/cafeteria/sales/report         - Satış raporu
GET    /api/v1/cafeteria/analytics            - En populer yemekler
```

---

## 1️⃣3️⃣ PARKING (Park Yönetimi Sistemi)

### 🎯 Amaç
Araç kaydı, park yeri yönetimi, plaka tanıma (ANPR) ve ücretlendirme.

### 📋 Ana Sorumlulukları
- Araç kaydı ve tanımlama
- Park yeri ataması (Rezerve, Uzun süreli)
- Plaka tanıma (ANPR) entegrasyonu
- Giriş-çıkış kayıt
- Ücretlendirme (Saatlik, Günlük, Aylık abonelik)
- Park yeri doluluk raporları

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Park Yerleri** | Öğrenci, Personel, Ziyaretçi, Engelli |
| **Plaka Tanıma** | AI ANPR sistemi (otomatik tanı) |
| **Ücretlendirme** | Saatlik (₺5), Aylık abonelik (₺500) |
| **Raporlama** | Doluluk oranı, gelir analizi |
| **Engelli Alanlar** | Uzun süreli, adanmış |

### 🔗 Bağımlılıkları
- 🔄 Wallet (Ödeme sistemi)
- 🔄 AccessControl (Giriş-çıkış)

### 💾 Temel Entity'ler
```
ParkingLot:
  - ParkingLotId (PK)
  - ParkingLotCode (Unique)
  - ParkingLotName
  - Location
  - TotalSpaces
  - LotType (Student, Staff, Visitor, Mixed, Disabled)
  - HasANPR

VehicleRegistration:
  - VehicleId (PK)
  - PersonId (FK)
  - LicensePlate (Unique)
  - Brand / Model / Color
  - RegistrationDate

ParkingCard:
  - ParkingCardId (PK)
  - VehicleId (FK)
  - CardType (Monthly, Semester, Annual, Daily)
  - IssueDate / ExpiryDate
  - Status

EntryExitLog:
  - LogId (PK)
  - VehicleId (FK)
  - ParkingLotId (FK)
  - EntryTime
  - ExitTime (nullable)
  - Duration (minutes, nullable)
  - Fee
  - PaymentStatus (Paid, Unpaid)
  - DetectionMethod (ANPR, Manual, Card)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/parking/vehicles/register      - Araç kaydet
POST   /api/v1/parking/entry                  - Park giriş (ANPR)
POST   /api/v1/parking/exit                   - Park çıkış (Ödeme)
GET    /api/v1/parking/lots/availability      - Park doluluk
GET    /api/v1/parking/revenue                - Gelir raporu
```

---

## 1️⃣4️⃣ FACILITY (Tesis Yönetimi)

### 🎯 Amaç
Binalar, odalar, laboratuvarlar, ekipman ve bakım yönetimi.

### 📋 Ana Sorumlulukları
- Bina ve oda yönetimi
- Laboratuvar yönetimi ve ekipman takibi
- Ekipman kalibrasyon planı
- Bakım talepleri ve planlama
- Enerji tüketimi takibi
- Kimyasal envanter (MSDS)
- Güvenlik kontrol

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Binalar** | Kat sayısı, toplam alan, asansör durumu |
| **Odalar** | Sınıf, Ofis, Lab, Toplantı, Çalışma odası |
| **Laboratuvarlar** | Bilgisayar, Kimya, Fizik, Biyoloji labs |
| **Ekipman** | Kalibrasyon, Garantisi, Kullanım kaydı |
| **Bakım** | Preventif, Düzeltici, Periyodik |
| **Enerji** | Elektrik, Su, Doğalgaz tüketimi |

### 🔗 Bağımlılıkları
- ✅ Core (Base entities)
- 🔄 Inventory (Ekipman stok)

### 💾 Temel Entity'ler
```
Building:
  - BuildingId (PK)
  - BuildingCode (Unique)
  - BuildingName
  - Address
  - TotalFloors / TotalRooms
  - ConstructionYear
  - HasElevator / HasDisabledAccess

Room:
  - RoomId (PK)
  - BuildingId (FK)
  - RoomCode (Unique)
  - RoomType (Classroom, Lab, Office, Meeting)
  - Capacity
  - Area
  - Facilities (Projector, Whiteboard, AC)

Laboratory:
  - LabId (PK)
  - LabCode (Unique)
  - LabName
  - LabType (Computer, Chemistry, Physics, Biology)
  - Capacity
  - ManagerId (FK)
  - SafetyLevel

LabEquipment:
  - EquipmentId (PK)
  - LaboratoryId (FK)
  - EquipmentCode (Unique)
  - EquipmentName
  - SerialNumber
  - LastCalibrationDate
  - NextCalibrationDate
  - Status

MaintenanceRequest:
  - RequestId (PK)
  - RequestNumber (Unique)
  - LocationId (Building/Room)
  - IssueType (Electrical, Plumbing, HVAC, IT)
  - Description
  - Priority (Low, Medium, High, Critical)
  - Status (New, Assigned, InProgress, Completed)
  - AssignedTo (FK)
  - CompletedDate (nullable)

EnergyConsumption:
  - ConsumptionId (PK)
  - BuildingId (FK)
  - ReadingDate
  - ElectricityKWh
  - WaterM3
  - NaturalGasM3
  - Costs
```

### 📡 Temel API Endpoints
```
POST   /api/v1/facility/buildings/register    - Bina tanımla
POST   /api/v1/facility/rooms/register        - Oda tanımla
POST   /api/v1/facility/labs/register         - Lab tanımla
POST   /api/v1/facility/maintenance/request   - Bakım talep
GET    /api/v1/facility/maintenance/status    - Bakım durumu
GET    /api/v1/facility/energy/report         - Enerji raporu
```

---

## 1️⃣5️⃣ HEALTH (Sağlık Hizmetleri)

### 🎯 Amaç
Öğrenci ve personel sağlık kayıtları, randevu yönetimi, aşı takibi ve iş sağlığı.

### 📋 Ana Sorumlulukları
- Sağlık kayıtları (Kan grubu, Kan basıncı, Alerjiler)
- Tıbbi randevu sistemi
- Reçete ve ilaç yönetimi
- Aşılama takibi
- Meslek hastalıkları raporlaması
- İş kazası kaydı
- Periyodik sağlık kontrolü

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Sağlık Kayıtları** | Kan grubu, Alerjiler, Kronik hastalıklar |
| **Randevu Sistemi** | Online randevu alma, SMS hatırlatması |
| **Reçeteler** | Doktor tarafından işleme konmuş reçeteler |
| **Aşılar** | Covid, Grip, Tetanoz, vb. takibi |
| **İSG** | Periyodik muayeneler, fit/unfit kararları |
| **İş Kazası** | Detaylı kayıt, rapor ve analiz |

### 🔗 Bağımlılıkları
- ✅ PersonMgmt (Kişi bilgileri)

### 💾 Temel Entity'ler
```
HealthRecord:
  - RecordId (PK)
  - PersonId (FK, Unique)
  - BloodType
  - Allergies
  - ChronicDiseases
  - CurrentMedications
  - EmergencyContact

MedicalAppointment:
  - AppointmentId (PK)
  - PersonId (FK)
  - AppointmentDate
  - DoctorName
  - Department
  - Reason
  - Status (Scheduled, Completed, Cancelled, NoShow)

Prescription:
  - PrescriptionId (PK)
  - PersonId (FK)
  - PrescriptionDate
  - DoctorName
  - Medications[]
  - Dosage[]
  - Diagnosis
  - Duration

VaccinationRecord:
  - VaccinationId (PK)
  - PersonId (FK)
  - VaccineName
  - VaccinationDate
  - DoseNumber
  - NextDoseDate (nullable)
  - AdministeredBy
  - LotNumber

WorkAccident:
  - AccidentId (PK)
  - EmployeeId (FK)
  - AccidentNumber (Unique)
  - AccidentDate
  - Location
  - Description
  - InjuryType / BodyPart
  - Severity (Minor, Moderate, Major, Fatal)
  - DaysLost
  - WitnessNames
  - Treatment
```

### 📡 Temel API Endpoints
```
POST   /api/v1/health/records/update          - Sağlık kaydı güncelle
POST   /api/v1/health/appointments/schedule   - Randevu al
GET    /api/v1/health/appointments/user       - Randevularım
POST   /api/v1/health/vaccinations/record     - Aşı kaydı
GET    /api/v1/health/vaccinations            - Aşı durumum
POST   /api/v1/health/incidents/report        - İş kazası kaydet
```

---

## 1️⃣6️⃣ RESEARCH (Araştırma & Yayınlar Yönetimi)

### 🎯 Amaç
Araştırma projeleri, yayınlar, patentler ve akademik performans takibi.

### 📋 Ana Sorumlulukları
- Araştırma projesi yönetimi
- Yayın ve atıf takibi
- Patent başvurusu ve onay takibi
- Proje ekibi yönetimi
- Harcama takibi (Bütçe)
- H-indeks ve akademik puanlar
- Uluslararası işbirlikler

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Proje Türleri** | Internal, TÜBİTAK, EU, Sanayi, Uluslararası |
| **Yayın Türleri** | Journal (SCI/SSCI), Conference, Book, Patent |
| **Atıf İstatistikleri** | Google Scholar, Scopus, WoS |
| **H-Index** | Akademik başarı metriği |
| **Uluslararası** | Erasmus+, Horizon2020 projeleri |

### 🔗 Bağımlılıkları
- ✅ Academic (Akademik yapı)
- ✅ PersonMgmt (Araştırmacı bilgileri)

### 💾 Temel Entity'ler
```
ResearchProject:
  - ProjectId (PK)
  - ProjectCode (Unique)
  - ProjectTitle
  - ProjectType (Internal, TUBITAK, EU, Industry)
  - PrincipalInvestigatorId (FK)
  - StartDate / EndDate
  - TotalBudget / SpentBudget
  - FundingSource
  - Status (Proposed, Active, Completed)

Publication:
  - PublicationId (PK)
  - AuthorId (FK)
  - PublicationType (Journal, Conference, Book)
  - Title
  - Authors
  - JournalOrConference
  - Year
  - DOI / ISSN / ISBN
  - ImpactFactor
  - CitationCount
  - IndexName (SCI, SSCI, Scopus)

Patent:
  - PatentId (PK)
  - PatentNumber (Unique)
  - PatentTitle
  - Inventors[]
  - ApplicationDate / ApprovalDate
  - Country
  - Status (Applied, Pending, Granted)

ProjectExpense:
  - ExpenseId (PK)
  - ProjectId (FK)
  - ExpenseDate
  - Category (Equipment, Personnel, Travel)
  - Amount
  - Description
  - Status (Pending, Approved, Paid)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/research/projects/register     - Proje tanımla
POST   /api/v1/research/publications/add      - Yayın ekle
POST   /api/v1/research/patents/file          - Patent başvurusu
GET    /api/v1/research/publications/{userId} - Yayın listesi
GET    /api/v1/research/h-index/{userId}     - H-Index değeri
POST   /api/v1/research/expenses/track        - Harcama kaydet
```

---

## 1️⃣7️⃣ ACTIVITIES (Öğrenci Aktiviteleri & Etkinlikler)

### 🎯 Amaç
Öğrenci kulüpleri, etkinlik yönetimi, katılım kaydı ve sertifikasyon.

### 📋 Ana Sorumlulukları
- Kulüp tanımlama ve yönetim
- Etkinlik planlama (Akademik, Sosyal, Spor, Kültürel)
- Etkinlik kayıtları ve ödeme
- Katılım takibi
- Sertifika verme
- Bütçe yönetimi (Kulüp geliri/gideri)
- Üye istatistikleri

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Kulüpler** | Akademik, Sosyal, Spor, Sanat, İlgi Grupları |
| **Etkinlik Türleri** | Konferans, Workshop, Turnuva, Gezi |
| **Kayıt Sistemi** | Online kayıt, ücretli/ücretsiz etkinlikler |
| **Sertifikalar** | Katılım belgeleri otomatik oluşturulur |
| **Bütçe** | Kulüp geliri, harcamaları, raporları |

### 🔗 Bağımlılıkları
- ✅ Academic (Öğrenci bilgileri)
- 🔄 Wallet (Etkinlik ücretleri)

### 💾 Temel Entity'ler
```
Club:
  - ClubId (PK)
  - ClubCode (Unique)
  - ClubName
  - Description
  - EstablishedDate
  - AdvisorId (FK)
  - PresidentStudentId (FK)
  - MemberCount
  - BudgetAmount
  - Status (Active, Inactive)

ClubMember:
  - MemberId (PK)
  - ClubId (FK)
  - StudentId (FK)
  - JoinDate
  - Role (President, VP, Secretary, Treasurer, Member)
  - IsActive

Event:
  - EventId (PK)
  - EventCode (Unique)
  - EventName
  - EventType (Academic, Social, Sports, Cultural)
  - EventDate / EndDate
  - Location
  - Capacity
  - OrganizerClubId (FK)
  - RequiresRegistration / IsFree
  - Price (nullable)
  - Status (Planned, Active, Completed)

EventRegistration:
  - RegistrationId (PK)
  - EventId (FK)
  - PersonId (FK)
  - RegistrationDate
  - AttendanceStatus (Registered, Attended, NoShow)
  - PaymentStatus (Paid, Unpaid)
  - CertificateIssued
  - FeedbackRating (1-5, nullable)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/clubs/create                   - Kulüp oluştur
POST   /api/v1/clubs/members/join             - Kulübe katıl
POST   /api/v1/events/create                  - Etkinlik planla
POST   /api/v1/events/register                - Etkinliğe kayıt ol
GET    /api/v1/events/user/registered         - Katıldığım etkinlikler
POST   /api/v1/events/certificates/issue     - Sertifika ver
```

---

## 1️⃣8️⃣ ANNOUNCEMENTS (Duyurular & Anketler Sistemi)

### 🎯 Amaç
Tüm üniversiteye veya hedef kitleye bildirim ve anket yapma sistemi.

### 📋 Ana Sorumlulukları
- Duyuru yayınlama (Genel, Acil, Akademik, İdari)
- Hedef kitle seçimi (Rol bazlı, Departman bazlı)
- Anket oluşturma ve cevap toplama
- Duyuru arşivi
- Okunma takibi
- İstatistiksel analiz

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Duyuru Türleri** | Genel, Akademik, İdari, Acil |
| **Öncelik Seviyeleri** | Low, Normal, High, Critical |
| **Hedef Kitle** | Herkes, Öğrenciler, Personel, Rol bazlı |
| **Anket Türleri** | Çoktan seçmeli, Açık-uçlu, Puan |
| **Okunma** | Gördü/Görmedi takibi |
| **Arşiv** | Eski duyurulara erişim |

### 🔗 Bağımlılıkları
- ✅ PersonMgmt (Kullanıcı bilgileri)
- 🔄 Notification System (Bildirim gönderimi)

### 💾 Temel Entity'ler
```
Announcement:
  - AnnouncementId (PK)
  - Title
  - Content
  - AnnouncementType (General, Academic, Admin, Urgent)
  - Priority (Low, Normal, High, Critical)
  - TargetAudience (Everyone, Students, Staff, Specific Role)
  - PublishDate
  - ExpiryDate (nullable)
  - CreatedBy (FK)
  - ViewCount
  - ImagePath (nullable)
  - AttachmentPath (nullable)
  - IsActive

Survey:
  - SurveyId (PK)
  - SurveyTitle
  - Description
  - StartDate / EndDate
  - TargetAudience
  - CreatedBy (FK)
  - IsAnonymous
  - ResponseCount

SurveyQuestion:
  - QuestionId (PK)
  - SurveyId (FK)
  - QuestionText
  - QuestionType (MultipleChoice, Text, Rating, YesNo)
  - QuestionOrder
  - IsRequired
  - Options (JSON)

SurveyResponse:
  - ResponseId (PK)
  - SurveyId (FK)
  - QuestionId (FK)
  - RespondentId (FK, nullable if anonymous)
  - ResponseText
  - ResponseDate
```

### 📡 Temel API Endpoints
```
POST   /api/v1/announcements/publish          - Duyuru yayınla
GET    /api/v1/announcements                  - Duyuruları listele
POST   /api/v1/surveys/create                 - Anket oluştur
POST   /api/v1/surveys/respond                - Ankete cevap ver
GET    /api/v1/surveys/results                - Anket sonuçları
POST   /api/v1/announcements/mark-read        - Duyuru okundu işaretle
```

---

## 1️⃣9️⃣ DOCUMENTS (Belge Yönetimi Sistemi)

### 🎯 Amaç
Öğrenci belge talepleri (Transkript, Diploma, Onay belgesi) ve resmi yazışmalar.

### 📋 Ana Sorumlulukları
- Dijital belge talep sistemi
- Otomatik fatura ve ücret alma
- Belge üretimi ve onay workflow'u
- Teslimat yöntemi (Kargo, Elden, Dijital)
- Resmi yazışma arşivi
- E-imza entegrasyonu
- Belge geçmişi

### ✨ Temel Özellikler
| Belge Türü | İşlem |
|-----------|--------|
| **Öğrenci Belgesi** | Otomatik üretim, Ücretli |
| **Transkript** | İng/Türkçe, E-imzalı |
| **Diploma** | Onaylı Diploma kopyası |
| **Onay Belgesi** | Akademik durum belgesi |
| **Resmi Yazışmalar** | EBYS benzeri gelen/giden kayıt |

### 🔗 Bağımlılıkları
- ✅ Academic (Belge üretimi)
- ✅ PersonMgmt (Kişi bilgileri)
- 🔄 Finance (Ücret alma)

### 💾 Temel Entity'ler
```
DocumentRequest:
  - RequestId (PK)
  - RequestNumber (Unique)
  - RequestDate
  - RequestedBy (FK)
  - DocumentType (Certificate, Transcript, Diploma, Approval)
  - Purpose
  - Language (Turkish, English)
  - Quantity
  - DeliveryMethod (PickUp, Mail, Digital)
  - Status (Pending, Approved, Processing, Ready, Delivered)
  - Fee
  - PaymentStatus

OfficialCorrespondence:
  - CorrespondenceId (PK)
  - CorrespondenceNumber (Unique)
  - CorrespondenceType (Incoming, Outgoing, Internal)
  - Subject
  - FromEntity / ToEntity
  - CorrespondenceDate
  - Content
  - Priority (Low, Normal, High, Urgent)
  - Status (Draft, Sent, Received, Reviewed, Replied)
  - AssignedTo (FK)
  - DueDate
```

### 📡 Temel API Endpoints
```
POST   /api/v1/documents/request              - Belge talep et
GET    /api/v1/documents/requests/status      - Talep durumu
POST   /api/v1/documents/pay                  - Belge ücreti öde
GET    /api/v1/documents/track                - Belge takibi
POST   /api/v1/correspondence/register        - Yazışma kaydet
GET    /api/v1/correspondence/inbox           - Gelen yazışmalar
```

---

## 2️⃣0️⃣ PERFORMANCE (Performans Değerlendirme Sistemi)

### 🎯 Amaç
Personel performans değerlendirmesi, KPI takibi, hedef yönetimi.

### 📋 Ana Sorumlulukları
- Periyodik performans değerlendirmesi
- 360 derece değerlendirme (Yönetici, Arkadaş, Kendisi)
- KPI (Key Performance Indicator) tanımlama ve takibi
- Hedef belirleme ve izleme
- Performans skorları ve analizi
- Gelişim planı oluşturma
- Ödül ve disiplin kararları

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Değerlendirme Dönemleri** | Yarıyıl, Yıllık |
| **Kriterler** | Teknik, Davranışsal, Liderlik |
| **360 Değerlendirme** | Çok yönlü geri bildirim |
| **KPI** | Ölçülebilir hedefler ve ilerleme |
| **Skalalar** | 1-5 puan sistemi |

### 🔗 Bağımlılıkları
- 🔄 HR (Çalışan yönetimi)
- 🔄 Payroll (Performans bonusu)

### 💾 Temel Entity'ler
```
PerformanceReview:
  - ReviewId (PK)
  - EmployeeId (FK)
  - ReviewPeriodStart / ReviewPeriodEnd
  - ReviewDate
  - ReviewerId (FK)
  - OverallScore (1-5)
  - Strengths / AreasForImprovement
  - Goals
  - DevelopmentPlan
  - Status (Draft, Submitted, Reviewed, Acknowledged)

PerformanceCriteria:
  - CriteriaId (PK)
  - CriteriaName
  - Description
  - Category (Technical, Behavioral, Leadership)
  - Weight (%)
  - MaxScore (5.0)

PerformanceScore:
  - ScoreId (PK)
  - ReviewId (FK)
  - CriteriaId (FK)
  - Score (1-5)
  - Comments

KPI:
  - KPIId (PK)
  - KPIName
  - UnitOfMeasure
  - TargetValue
  - ActualValue
  - MeasurementPeriod (Monthly, Quarterly, Annual)
  - DepartmentId (FK)
  - ResponsiblePersonId (FK)
  - Status (InProgress, Achieved, NotAchieved)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/performance/reviews/create     - Değerlendirme yarat
POST   /api/v1/performance/scores/submit      - Puan ver
GET    /api/v1/performance/reviews/{empId}   - Değerlendirmeler
POST   /api/v1/performance/kpis/track         - KPI takip
GET    /api/v1/performance/analytics          - Performans analizi
```

---

## 2️⃣1️⃣ GRADUATION (Mezuniyet & Diploma Sistemi)

### 🎯 Amaç
Mezuniyet koşulları kontrolü, diploma üretimi ve apostil işlemleri.

### 📋 Ana Sorumlulukları
- Mezuniyet gereksinimleri tanımlama (GANO, Kredi, Kurs)
- Otomatik mezuniyet uygunluk kontrolü
- Diploma üretimi ve numaralandırması
- Apostil başvurusu yönetimi
- Diploma teslimat takibi
- Mezun bilgi sistemi

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Uygunluk Kontrolü** | GANO ≥ 2.0, Kredi ≥ 120 |
| **Onurları** | High Honors, Honors, Regular |
| **Diploma Dili** | Türkçe ve İngilizce |
| **Apostil** | 187 ülkeye geçerli noter onayı |
| **Teslimat** | Elden, Kargo, Dijital |

### 🔗 Bağımlılıkları
- ✅ Academic (Ders ve not verileri)
- ✅ PersonMgmt (Öğrenci bilgileri)

### 💾 Temel Entity'ler
```
GraduationRequirement:
  - RequirementId (PK)
  - ProgramId (FK)
  - MinimumGANO (2.0)
  - MinimumCredits (120)
  - RequiredCourses[] (Zorunlu dersler)
  - OtherRequirements

Diploma:
  - DiplomaId (PK)
  - DiplomaNumber (Unique)
  - StudentId (FK)
  - ProgramId (FK)
  - GraduationDate
  - GANO
  - Honors (None, Honors, HighHonors)
  - DegreeTitle (Mühendis, Doktor, vb.)
  - IssueDate
  - IssuedBy (FK)
  - Status (Pending, Issued, Delivered)
  - DeliveryDate (nullable)

Apostille:
  - ApostilleId (PK)
  - DiplomaId (FK)
  - RequestDate
  - TargetCountry
  - ApostilleNumber (nullable)
  - IssueDate (nullable)
  - Status (Requested, InProgress, Completed)
  - Cost
```

### 📡 Temel API Endpoints
```
POST   /api/v1/graduation/check-eligibility  - Uygunluk kontrol
POST   /api/v1/graduation/generate-diploma   - Diploma üret
POST   /api/v1/apostille/request             - Apostil talep
GET    /api/v1/graduation/diplomas/{studentId} - Diplomalar
GET    /api/v1/graduation/status             - Mezuniyet durumu
```

---

## 2️⃣2️⃣ SCHOLARSHIPS (Burs & Mali Yardım Sistemi)

### 🎯 Amaç
Burs tanımlama, başvuru kabul, değerlendirme ve mali yardım yönetimi.

### 📋 Ana Sorumlulukları
- Burs tanımlama ve yönetimi
- Başvuru kabul ve kriteerleri kontrolü
- Burs değerlendirmesi ve onay
- Mali yardım (Sosyal, Acil, Yemek vb.)
- Aylık/Yarıyıllık ödeme
- Başarı koşulları takibi

### ✨ Temel Özellikler
| Burs Türü | Kriterleri |
|----------|-----------|
| **Akademik** | GANO ≥ 3.5 |
| **Mali** | Aile geliri ≤ 2xSSO |
| **Spor** | Sporcu belgesi, başarı |
| **Sanat** | Sanatsal başarı belgeleri |
| **Sosyal Yardım** | Acil ihtiyaç (Gıda, Kitap) |

### 🔗 Bağımlılıkları
- ✅ Academic (Öğrenci bilgileri)
- 🔄 Finance (Ödeme sistemi)

### 💾 Temel Entity'ler
```
Scholarship:
  - ScholarshipId (PK)
  - ScholarshipCode (Unique)
  - ScholarshipName
  - ScholarshipType (Academic, Financial, Sports, Art)
  - Amount
  - DurationSemesters
  - Criteria (Açıklama)
  - MinimumGANO (nullable)
  - AvailableSlots
  - UsedSlots
  - ApplicationStartDate / ApplicationEndDate

ScholarshipApplication:
  - ApplicationId (PK)
  - ScholarshipId (FK)
  - StudentId (FK)
  - ApplicationDate
  - GANO
  - FamilyIncome (nullable)
  - Status (Pending, Approved, Rejected)
  - ReviewedBy (FK)
  - ReviewDate (nullable)
  - AwardedAmount (nullable)
  - AwardedSemesters

FinancialAid:
  - AidId (PK)
  - StudentId (FK)
  - AidType (Emergency, Food, Books, Transportation)
  - Amount
  - AidDate
  - Reason
  - Status (Pending, Approved, Disbursed)
  - ApprovedBy (FK)
  - DisbursementDate (nullable)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/scholarships/apply             - Burs başvuru
GET    /api/v1/scholarships/available         - Uygun burslar
GET    /api/v1/scholarships/my-scholarships   - Aldığım burslar
POST   /api/v1/financial-aid/request          - Mali yardım talep
GET    /api/v1/financial-aid/status           - Yardım durumu
```

---

## 2️⃣3️⃣ TECHNICAL (Teknik Servis & Bakım Yönetimi)

### 🎯 Amaç
Bakım taleplerinin yönetimi, preventif bakım planlaması, enerji tüketimi takibi.

### 📋 Ana Sorumlulukları
- Bakım talepleri (Ticket sistem)
- Preventif bakım planlama
- Periyodik kontrol ve muayeneler
- Enerji tüketimi raporları
- Ekipman ömrü takibi
- Bakım maliyetlerinin analizi

### ✨ Temel Özellikler
| Bakım Türü | Amaç |
|-----------|------|
| **Preventif** | Arıza öncesi, planlanmış |
| **Düzeltici** | Arızadan sonra, acil |
| **Periyodik** | Rutin kontroller (Aylık, Yıllık) |
| **Enerji Kontrolü** | Tüketim analizi, tasarruf |

### 🔗 Bağımlılıkları
- 🔄 Facility (Tesis yönetimi)

### 💾 Temel Entity'ler
```
MaintenanceRequest:
  - RequestId (PK)
  - RequestNumber (Unique)
  - RequestDate
  - RequestedBy (FK)
  - Location
  - IssueType
  - Description
  - Priority (Low, Medium, High, Critical)
  - Status (New, Assigned, InProgress, Completed)
  - AssignedTo (FK)
  - CompletedDate (nullable)
  - Cost (nullable)

MaintenanceSchedule:
  - ScheduleId (PK)
  - EquipmentName
  - Frequency (Daily, Weekly, Monthly, Quarterly, Annual)
  - LastMaintenanceDate (nullable)
  - NextMaintenanceDate
  - ResponsiblePerson (FK)
  - Checklist
  - Status

PeriodicMaintenancePlan:
  - PlanId (PK)
  - PlanName
  - Frequency
  - Checklist[]
  - RequiredTools[]
  - RequiredMaterials[]
```

### 📡 Temel API Endpoints
```
POST   /api/v1/maintenance/request            - Bakım talep
GET    /api/v1/maintenance/requests/status    - Talep durumu
GET    /api/v1/maintenance/schedule           - Bakım takvimi
GET    /api/v1/maintenance/analytics          - Bakım raporları
```

---

## 2️⃣4️⃣ IT (BİLGİ TEKNOLOJİLERİ YÖNETİMİ)

### 🎯 Amaç
Sunucu, ağ cihazları, yazılım lisansları ve HelpDesk ticket sistemi.

### 📋 Ana Sorumlulukları
- Sunucu yönetimi (Web, Database, Application, Mail)
- Ağ cihazları yönetimi (Router, Switch, Firewall)
- Yazılım lisans takibi
- HelpDesk ticket sistemi
- Teknik destek ve problem çözme
- IT altyapı raporları
- Sistem performans monitoring

### ✨ Temel Özellikler
| Özellik | Açıklama |
|---------|----------|
| **Sunucular** | OS, CPU, RAM, Storage tracking |
| **Ağ Cihazları** | IP, MAC, Firmware, Status |
| **Lisanslar** | Yazılım, Abonelik, Expiry |
| **Tickets** | Kategori, Öncelik, SLA |
| **Monitoring** | CPU, Memory, Disk, Network |

### 🔗 Bağımlılıkları
- ✅ Core (Base entities)

### 💾 Temel Entity'ler
```
Server:
  - ServerId (PK)
  - ServerName (Unique)
  - IPAddress
  - ServerType (Web, Database, Application, Mail)
  - OperatingSystem
  - CPU / RAM / Storage
  - Location
  - PurchaseDate
  - WarrantyExpiry (nullable)
  - Status (Active, Inactive, Maintenance, Decommissioned)

SoftwareLicense:
  - LicenseId (PK)
  - SoftwareName
  - Version
  - LicenseKey
  - LicenseType (Perpetual, Subscription, Volume)
  - PurchaseDate
  - ExpiryDate (nullable)
  - TotalLicenses
  - UsedLicenses
  - Cost
  - Vendor

HelpDeskTicket:
  - TicketId (PK)
  - TicketNumber (Unique)
  - CreatedBy (FK)
  - Category (Hardware, Software, Network, Account)
  - Priority (Low, Medium, High, Critical)
  - Subject
  - Description
  - Status (New, Open, InProgress, Pending, Resolved, Closed)
  - AssignedTo (FK)
  - FirstResponseDate (nullable)
  - ResolvedDate (nullable)
  - ClosedDate (nullable)
  - Resolution
  - UserSatisfaction (1-5, nullable)
```

### 📡 Temel API Endpoints
```
POST   /api/v1/it/tickets/create              - Support ticket oluştur
GET    /api/v1/it/tickets/{ticketId}         - Ticket detayı
PUT    /api/v1/it/tickets/{ticketId}/update  - Ticket güncelle
GET    /api/v1/it/servers/status              - Sunucu durumu
GET    /api/v1/it/licenses/expiring           - Yakında expire olacaklar
GET    /api/v1/it/system/health               - Sistem sağlığı
```

---

## 📊 ÖZET TABLOSU

| # | MODÜL | BAĞLILIK | AMAÇ | ETKI |
|---|-------|----------|------|------|
| 1 | Wallet | Low | Dijital cüzdan & kart | 🔴 Kritik |
| 2 | VirtualPOS | Medium | Ödeme gateway'leri | 🔴 Kritik |
| 3 | StudentFees | Medium | Harç yönetimi | 🔴 Kritik |
| 4 | Leave | Low | İzin yönetimi | 🟡 Yüksek |
| 5 | HR | Low | Personel yönetimi | 🟡 Yüksek |
| 6 | Payroll | High | Bordro (🇹🇷) | 🔴 Kritik |
| 7 | Finance | High | Muhasebe | 🔴 Kritik |
| 8 | AccessControl | Medium | Güvenlik & Erişim | 🔴 Kritik |
| 9 | Procurement | Medium | Satın alma | 🟡 Yüksek |
| 10 | Inventory | Medium | Stok yönetimi | 🟡 Yüksek |
| 11 | Library | Low | Kütüphane | 🟡 Yüksek |
| 12 | Cafeteria | Low | Yemekhane/Kafeterya | 🟡 Yüksek |
| 13 | Parking | Low | Park yönetimi | 🟡 Orta |
| 14 | Facility | Low | Tesis yönetimi | 🟡 Orta |
| 15 | Health | Low | Sağlık hizmetleri | 🟡 Orta |
| 16 | Research | Low | Araştırma yönetimi | 🟢 Düşük |
| 17 | Activities | Low | Etkinlikler & Kulüpler | 🟢 Düşük |
| 18 | Announcements | Low | Duyuru sistemi | 🟢 Düşük |
| 19 | Documents | Low | Belge yönetimi | 🟢 Düşük |
| 20 | Performance | Medium | Performans değ. | 🟡 Orta |
| 21 | Graduation | Medium | Mezuniyet & Diploma | 🟡 Yüksek |
| 22 | Scholarships | Low | Burs & Yardım | 🟢 Düşük |
| 23 | Technical | Low | Bakım & Servis | 🟡 Orta |
| 24 | IT | Low | IT Yönetimi | 🟡 Orta |

---

## 🎯 BAŞLAMA SIRASI (ÖNERİLEN)

1. ✅ **Wallet** → Temel ödeme sistemi
2. → **VirtualPOS** → Gateway entegrasyonu
3. → **StudentFees** → Harç yönetimi
4. → **Leave + HR** → İdari altyapı
5. → **Payroll** → Bordro (🇹🇷 kritik)
6. → **Finance** → Muhasebe
7. → **AccessControl** → Güvenlik
8. → **Procurement + Inventory** → Alım-satım
9. → **Library + Cafeteria + Parking** → Hizmetler
10. → **Facility + Health + Research** → Destekleyici
11. → **Announcements + Documents + Performance + Graduation + Scholarships + Technical + IT** → Yönetim

**TOTAL: 24 hafta (~6 ay)**

---

**Sorularınız varsa, her modülün detaylarını daha derinlemesine açıklayabilirim!** 🚀
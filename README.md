# KAPSAMLı ÜNİVERSİTE YÖNETİM SİSTEMİ
## PROJE TASLAĞI

**Versiyon:** 1.0  
**Tarih:** 7 Ekim 2025  
**Proje Adı:** Kapsamlı Üniversite Yönetim Sistemi  
**Teknoloji:** .NET 9.0, EF Core 9.0, SQL Server 2022

---

## İÇİNDEKİLER

1. [Sistem Genel Bakış](#1-sistem-genel-bakış)
2. [Portal Sistemleri - Detaylı](#2-portal-sistemleri)
3. [Modüller ve Alt Sistemler](#3-modüller-ve-alt-sistemler)
4. [Teknik Mimari](#4-teknik-mimari)
5. [Veritabanı Yapısı](#5-veritabanı-yapısı)
6. [API Endpoint'leri](#6-api-endpointleri)
7. [Güvenlik ve Yetkilendirme](#7-güvenlik-ve-yetkilendirme)
8. [Entegrasyonlar](#8-entegrasyonlar)
9. [Deployment Stratejisi](#9-deployment-stratejisi)

---

## 1. SİSTEM GENEL BAKIŞ

### 1.1 Sistem Amacı

Üniversitenin tüm akademik, idari ve operasyonel süreçlerini dijitalleştiren, her kullanıcı tipine özel portal sunabilen, entegre, ölçeklenebilir ve güvenli bir platform.

### 1.2 Temel Prensipler

- **SOLID Prensipleri:** Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **Clean Architecture:** Katmanlı mimari (Domain, Application, Infrastructure, Presentation)
- **Domain-Driven Design (DDD):** İş mantığı merkezli tasarım
- **CQRS Pattern:** Command/Query ayrımı, optimum performans
- **Repository Pattern:** Veri erişim soyutlaması
- **Unit of Work Pattern:** Transaction yönetimi
- **Dependency Injection:** Loose coupling, test edilebilirlik
- **Event-Driven Architecture:** Modüller arası asenkron iletişim
- **Responsive Design:** Mobil uyumlu, her cihazda çalışan arayüz

### 1.3 Kullanıcı Rolleri ve Yetkileri

1. **Süper Admin** - Sistem yöneticisi (tam yetki)
2. **Rektör / Rektör Yardımcıları** - Stratejik yönetim
3. **Genel Sekreter** - İdari koordinasyon
4. **Dekan / Dekan Yardımcıları** - Fakülte yönetimi
5. **Bölüm Başkanları** - Bölüm yönetimi
6. **Akademik Personel** - Eğitim ve araştırma
   - Profesör
   - Doçent
   - Dr. Öğretim Üyesi
   - Öğretim Görevlisi
   - Araştırma Görevlisi
7. **İdari Personel** - İdari işlemler
   - Müdür, Şef, Memur, Sekreter
8. **Öğrenciler** - Eğitim alan kullanıcılar
   - Lisans, Yüksek Lisans, Doktora
9. **Özel Rol Kullanıcıları**
   - Muhasebe, Satın Alma, İnsan Kaynakları
   - Güvenlik, Kütüphane, Yemekhane/Kafeterya
   - IT Destek Personeli
10. **Misafir Kullanıcılar**
    - Ziyaretçi, Konuk Öğretim Üyesi, Stajyer

---

## 2. PORTAL SİSTEMLERİ - DETAYLI

### 2.1 ÖĞRENCİ PORTALI

**Portal URL:** students.university.edu

#### 2.1.1 Ana Sayfa Dashboard

**Üst Başlık Bilgileri:**
- Profil fotoğrafı
- Hoşgeldin mesajı (Ad Soyad)
- Öğrenci numarası
- Bölüm ve sınıf bilgisi
- Çıkış, Ayarlar, Bildirimler, Mesajlar

**Hızlı İstatistikler (Büyük Kartlar):**
- **Bakiye:** Güncel bakiye tutarı + Hızlı yükleme butonu
- **Genel Ortalama:** GANO + Detay linki
- **Devamsızlık:** Yüzde ve uyarı durumu
- **Bekleyen Ödeme:** Toplam borç tutarı

**Bugünün Ders Programı:**
- Tarih ve gün bilgisi
- Her ders için:
  - Saat aralığı
  - Ders adı
  - Öğretim üyesi
  - Derslik bilgisi
  - Ders materyalleri linki

**Son Aktiviteler ve Bildirimler:**
- Yeni not girişi bildirimleri
- Bütçe uyarıları
- Yeni ödev/proje bildirimleri
- Ödeme hatırlatmaları

**Yaklaşan Etkinlikler:**
- Sınavlar (geri sayım ile)
- Proje teslim tarihleri
- Ödeme son tarihleri
- Kampüs etkinlikleri

---

#### 2.1.2 Dijital Kartım

**Kart Görünümü (3D Kart Tasarımı):**

Dijital kart üzerinde:
- Üniversite logosu
- Öğrenci fotoğrafı
- Ad Soyad
- Öğrenci numarası
- Bölüm bilgisi
- Fakülte bilgisi
- Kart numarası
- Geçerlilik tarihi
- **QR Kod** (büyük ve net - geçiş sistemleri için)
- Barkod (alternatif okuma)

**QR Kod Kullanım Alanları:**
- Kampüs giriş/çıkış turnikeleri
- Bina ve sınıf giriş kontrolleri
- Kütüphane giriş ve kitap ödünç alma
- Yemekhane ve kafeterya ödemeleri
- Laboratuvar erişim kontrolü
- Sınav salon giriş kontrolü
- Spor salonu ve sosyal tesisler
- Otomatik kapı açma sistemleri

**Kart İşlemleri:**
- Kartı Tam Ekran Göster
- Kartı PDF olarak İndir
- Apple Wallet'a Ekle
- Google Pay'e Ekle
- Kartımı Kaybettim (Bloke Et)
- Yeni Kart Talep Et
- Kart Geçmişi

---

#### 2.1.3 Akademik Bilgilerim

##### A) Notlarım ve Transkript

**Aktif Dönem Notları:**

Tablo formatında:
- Ders Kodu
- Ders Adı
- Vize Notu
- Final Notu
- Bütünleme Notu
- Harf Notu
- Durum (Devam Ediyor/Geçti/Kaldı)

Dönem vize ortalaması hesaplanır ve gösterilir.

**Geçmiş Dönemler:**
- Dönem seçici (dropdown)
- Her dönem için notlar
- Dönemlik ortalamalar

**Transkript Özeti:**
- Genel Not Ortalaması (GANO): X.XX / 4.00
- Son Dönem Ortalaması (YANO): X.XX / 4.00
- Toplam Alınan Kredi: XXX AKTS
- Toplam Geçilen Kredi: XXX AKTS
- Kalan Ders Sayısı
- Mezuniyet için Kalan Kredi

**İndirme Seçenekleri:**
- Transkript İndir (Türkçe PDF - Resmi Onaylı)
- Transcript (English PDF - Official)
- Not Analizi ve Grafik
- Mezuniyet Tahmini

**Not Grafiği:**
- Dönemsel not trendi (line chart)
- Y ekseni: Not ortalaması (0-4.00)
- X ekseni: Dönemler (1.1, 1.2, 2.1, ...)
- Trend analizi: Yükseliş/Düşüş/Stabil
- Hedef karşılaştırması

**Not İtiraz Sistemi:**
- İtiraz Et butonu
- İtiraz süresi bilgisi
- İtiraz formu

---

##### B) Sınav Takvimim

**Yaklaşan Sınavlar (Öncelikli Gösterim):**

Her sınav için büyük kart:
- Ders adı
- Sınav türü (Vize/Final/Quiz/Bütünleme)
- Tarih ve saat
- Salon ve sıra numarası
- **Geri Sayım** (X gün Y saat Z dakika kaldı)
- Sınav kapsamı
- Öğretim üyesi
- Butonlar:
  - Ders Notlarını Görüntüle
  - Çalışma Planı Oluştur
  - Salon Yerleşim Planı
  - Hatırlatıcı Kur

**Tüm Sınavlar:**

**Görünüm Seçenekleri:**
- Liste Görünümü
- Takvim Görünümü (aylık)
- Haftalık Görünüm

**Takvim Görünümü:**
- Aylık takvim
- Sınav günleri işaretli
- Tıklanabilir sınav detayları

**Liste Görünümü:**

Tablo formatında:
- Tarih
- Ders Adı
- Sınav Türü
- Saat
- Salon
- Durum

**Sınav Hatırlatıcı Ayarları:**
- Sınava 1 hafta kala e-posta
- Sınava 3 gün kala SMS
- Sınava 1 gün kala push notification
- Sınav günü sabah hatırlatma
- Sınav saatinden 1 saat önce

**Geçmiş Sınavlar:**
- Tarih
- Ders
- Not
- Durum (Geçti/Kaldı)

---

##### C) Ders Programım

**Haftalık Ders Programı (Tablo Görünümü):**

Tablo yapısı:
- Satırlar: Saat dilimleri (09:00-10:50, 11:00-12:50, vb.)
- Sütunlar: Günler (Pazartesi - Pazar)
- Hücreler: Ders bilgileri
  - Ders adı
  - Derslik
  - Öğretim üyesi

Program altında:
- Toplam ders saati/hafta
- Toplam AKTS kredi
- İndirme seçenekleri:
  - PDF İndir
  - Excel İndir
  - Takvime Aktar (iCal)
  - Yazdır

**Bugünün Dersleri (Detaylı Görünüm):**

Her ders için kart:
- Saat aralığı
- Ders adı ve kodu
- Derslik (bina + salon)
- Öğretim üyesi (ad + iletişim)
- Bugünkü konu
- Ders materyalleri linkleri
- Geri sayım (X saat Y dakika kaldı)

**Ders Detay Sayfası:**

Her ders için:
- Genel Bilgiler:
  - Öğretim üyesi
  - Kredi bilgisi (teorik/pratik/AKTS)
  - Ön koşul dersleri
- Syllabus (Ders İçeriği):
  - Haftalık konular listesi
- Kaynaklar:
  - Ana kaynaklar
  - Önerilen kaynaklar
- Ders Materyalleri:
  - Sunumlar (PDF/PPT)
  - Lab kodları
  - Ödevler
  - Videolar
- Değerlendirme:
  - Vize oranı
  - Final oranı
  - Ödev/Proje oranı
  - Lab/Quiz oranı

**Devam Durumu:**

Tablo formatında:
- Ders Adı
- Devamsız Saat
- Maksimum İzin Verilen
- Kalan Hak
- Durum (İyi/Dikkat/Tehlikeli)

Uyarılar:
- %20'yi aşan dersler için uyarı
- %30'a yaklaşanlar için kırmızı uyarı

**Mazeret Bildirimi:**
- Mazeret Bildir butonu
- Rapor/belge yükleme
- Onay takibi

---

#### 2.1.4 Giriş-Çıkış Kayıtlarım

**Son 30 Gün Özeti:**
- Toplam giriş sayısı
- Ortalama kampüste kalma süresi (saat/gün)
- En erken giriş saati
- En geç çıkış saati
- En çok kullanılan giriş noktası

**Grafik Gösterim:**
- Günlük giriş-çıkış grafiği (line chart)
- X ekseni: Günler (Pazartesi-Pazar)
- Y ekseni: Saat
- 2 çizgi: Giriş saati, Çıkış saati

**Detaylı Kayıt Listesi:**

Tablo formatında:
- Tarih
- Saat
- Tür (Giriş/Çıkış)
- Lokasyon (hangi kapı/turnike)
- Cihaz (QR Kod/Kart/Biyometrik)

Filtreleme:
- Tarih aralığı seçici
- Lokasyon filtresi
- Tür filtresi

İndirme:
- Excel İndir
- PDF Rapor

**Lokasyon Bazlı Geçmiş:**

Bar chart:
- En çok kullanılan 5 lokasyon
- Geçiş sayıları
- Yüzde dağılımı

**Aylık Özet:**
- Toplam kampüste geçirilen süre
- Günlük ortalama
- En aktif gün
- En az aktif gün
- Sınıf ortalaması ile karşılaştırma

---

#### 2.1.5 Harcama ve Yükleme Kayıtlarım

**Mali Durum Özeti:**

Büyük kart görünümü:
- **Güncel Bakiye** (büyük font)
- Son işlem bilgisi
- Butonlar:
  - Bakiye Yükle
  - Detaylı Rapor
  - Bütçe Planlayıcı

**Harcama Geçmişi (Son 30 Gün):**

Tablo formatında:
- Tarih
- Saat
- Açıklama (nereden, ne)
- Tutar (eksi ile)
- Kalan Bakiye

Filtreleme ve sıralama seçenekleri

İndirme:
- Daha Fazla Göster
- Filtrele (tarih, tutar, kategori)
- Excel İndir
- PDF Rapor

**Yükleme Geçmişi:**

Tablo formatında:
- Tarih
- Saat
- Yöntem (Kredi Kartı/Havale/Nakit)
- Tutar (artı ile)
- Durum (Başarılı/Bekliyor/İptal)

Toplam yükleme tutarı gösterilir.

**Harcama İstatistikleri:**

Kart görünümü:
- Toplam harcama (dönem/ay)
- Günlük ortalama harcama

**Kategori Bazlı Harcama (Bar Chart):**
- Yemekhane: XX% (tutar)
- Kafeterya: XX% (tutar)
- Kırtasiye: XX% (tutar)
- Fotokopi/Baskı: XX% (tutar)
- Kütüphane: XX% (tutar)
- Diğer: XX% (tutar)

**En Çok Harcama Yapılan Yerler:**
1. Yemekhane Merkez - ₺XXX
2. Kafeterya A Blok - ₺XXX
3. Kafeterya B Blok - ₺XXX

**Öneri Sistemi:**
- Geçen aya göre değişim analizi
- Tasarruf önerileri

**Aylık Karşılaştırma Grafiği:**
- Bar chart
- Son 6 ayın harcamaları
- Trend analizi

**Bütçe Planlayıcı:**

Interaktif bölüm:
- Aylık bütçe belirleme
- Kalan bütçe gösterimi
- Progress bar (harcama yüzdesi)
- Günlük harcama limiti hesaplama
- Bugünkü harcama vs limit
- Tasarruf önerileri

**Harcama Uyarıları:**
- Günlük limit aşımı uyarısı
- Haftalık bütçe uyarısı
- Ay sonu projeksiyonu

**Bakiye Yükleme Ekranı:**

Hızlı tutar seçenekleri:
- ₺50, ₺100, ₺150, ₺200, ₺250
- Özel tutar

Ödeme yöntemi seçimi:
- Kredi Kartı / Banka Kartı (Anında)
- Havale / EFT (1-2 iş günü)
- Nakit (Gişeden yükle)

---

#### 2.1.6 Mesajlaşma ve İletişim

**Gelen Kutusu:**
- Okunmamış mesaj sayısı
- Mesaj listesi:
  - Gönderen
  - Konu
  - Önizleme
  - Tarih/Saat
  - Okundu işareti

**Mesaj Detayı:**
- Gönderen bilgisi
- Tarih ve saat
- Konu
- Mesaj içeriği
- Ekler (varsa)
- Butonlar:
  - Yanıtla
  - İlet
  - Sil
  - Yazdır

**Yeni Mesaj Oluştur:**
- Alıcı seçimi
- Konu
- Mesaj içeriği
- Ek dosya yükleme

---

#### 2.1.7 Duyurular ve Etkinlikler

**Güncel Duyurular:**

Öncelik sırasına göre:
- 🔴 Acil duyurular
- 🔵 Genel duyurular
- 🟢 Bilgilendirme duyuruları

Her duyuru için:
- Başlık
- Tarih
- Özet
- Detay linki

**Etkinlikler:**
- Başlık
- Tarih ve saat
- Lokasyon
- Açıklama
- Kayıt butonu (kayıt gerektiriyorsa)

---

#### 2.1.8 Diğer Özellikler

**Öğrenci İşleri:**
- Belge Talep:
  - Öğrenci belgesi
  - Transkript
  - Onay belgesi
  - Diploma örneği
- Dilekçe Oluşturma ve Takip
- Harç Ödeme
- Kayıt Yenileme
- Ders Kayıt İşlemleri

**Kütüphane:**
- Kitap Arama ve Rezervasyon
- Ödünç Aldığım Kitaplar
- Gecikmeler ve Cezalar
- E-kitap Erişimi
- Tez Arşivi Erişimi

**Sosyal Aktiviteler:**
- Kulüp Üyeliklerim
- Etkinlik Kayıtlarım
- Spor Salonu Rezervasyonu
- Sosyal Tesis Kullanımı

**Destek ve Yardım:**
- IT Destek Talebi Oluştur
- Öğrenci İşleri Destek
- SSS (Sık Sorulan Sorular)
- Chatbot Asistan
- Canlı Destek

---

### 2.2 ÖĞRETİM ÜYESİ PORTALI

**Portal URL:** staff.university.edu

#### 2.2.1 Ana Sayfa Dashboard

**Üst Başlık Bilgileri:**
- Profil fotoğrafı
- Hoşgeldin mesajı
- Sicil numarası
- Bölüm bilgisi
- Çıkış, Ayarlar, Bildirimler, Mesajlar

**Hızlı İstatistikler:**
- **Bu Hafta Ders Saati:** XX saat
- **Danışman Öğrenci:** XX öğrenci
- **Not Girişi Bekliyor:** X sınıf
- **Bakiye:** ₺XXX.XX

**Bugünün Ders Programı:**

Her ders için:
- Saat aralığı
- Ders adı ve kodu
- Sınıf
- Öğrenci sayısı
- Derslik
- Bugünkü konu
- Butonlar:
  - Yoklama Al
  - Materyaller
  - Not Gir

**Danışmanlık Saati:**
- Saat
- Lokasyon
- Randevulu öğrenci sayısı
- Randevuları Gör butonu

**Bekleyen İşler:**
- 🔴 Acil: Not girişi son tarihi yaklaşan
- 🟡 Danışman onayı bekleyen öğrenciler
- 🟡 Yanıtlanmamış mesajlar
- 🟢 Yaklaşan proje teslim tarihleri

---

#### 2.2.2 Derslerim ve Ders Programı

**Verdiğim Dersler (Aktif Dönem):**

Tablo formatında:
- Ders Kodu
- Ders Adı
- Sınıf
- Öğrenci Sayısı
- Haftalık Saat
- Derslik
- Durum
- Butonlar:
  - Detaya Git
  - Yoklama
  - Not Girişi
  - Materyaller

Altında toplam:
- Toplam ders saati/hafta
- Toplam öğrenci sayısı

**Haftalık Ders Programım:**

Tablo yapısı:
- Satırlar: Saat dilimleri
- Sütunlar: Günler
- Hücreler: Ders bilgileri
  - Ders adı
  - Öğrenci sayısı
  - Derslik

İndirme seçenekleri

---

#### 2.2.3 Ders Detayları ve Konu Takvimi

**Ders Genel Bilgileri:**
- Ders adı ve kodu
- Dönem
- Öğrenci sayısı
- Kredi bilgisi
- Derslik bilgileri
- Ön koşul dersleri
- Butonlar:
  - Syllabus
  - Öğrenci Listesi
  - Yoklama
  - Notlar
  - Materyaller

**Haftalık Ders Konuları (Syllabus):**

Tablo formatında:
- Hafta
- Tarih
- Konu
- Durum (İşlendi/Şimdi/Gelecek)
- Materyaller (linkler)
- Düzenleme butonları

**Bu Haftanın Konusu Detayı:**

Büyük kart görünümü:
- Hafta numarası ve konu başlığı
- **Anlatılacak Konular:**
  - Alt başlıklar
  - Detaylar
  - Örnek uygulamalar
- **Ders Materyalleri:**
  - Sunum dosyaları
  - Kod örnekleri
  - Lab klavuzları
  - Video anlatımlar
- **Ödevler ve Değerlendirmeler:**
  - Quiz bilgisi
  - Ödev detayları
- **Önerilen Kaynaklar:**
  - Kitaplar
  - Bölümler
- Butonlar:
  - Materyalleri Düzenle
  - Yeni Materyal Ekle
  - Öğrencilere Bildir

**Materyal Yönetimi:**

Dosya/klasör yapısı:
- Her hafta için klasör
- Her klasörde:
  - Dosya listesi
  - İndirme istatistikleri
  - Düzenleme/Silme butonları

Yeni materyal yükleme:
- Dosya yükle
- Klasör oluştur
- Link ekle

---

#### 2.2.4 Not Girişi ve Değerlendirme

**Not Girişi Bekleyen Sınavlar:**

Kart görünümü:
- 🔴 Acil (son tarihe yakın)
- 🟡 Normal
- Ders adı
- Sınav türü
- Son giriş tarihi
- Öğrenci sayısı
- Durum (girilen/toplam)
- Butonlar:
  - Not Gir
  - Toplu Yükleme

**Not Girişi Ekranı:**

Üst bilgiler:
- Ders bilgisi
- Sınav bilgisi
- Son giriş tarihi
- Geri sayım

Araçlar:
- Excel'den Aktar
- Not Şablonu İndir
- Otomatik Kaydet

Tablo:
- Sıra No
- Öğrenci No
- Ad Soyad
- Not (input field)
- Harf Notu (otomatik hesaplanan)
- Durum (Geçti/Kaldı)

Altında:
- İlerleme (X/Y öğrenci)
- Sınıf ortalaması
- Kaydet ve Onayla butonları

**Not İstatistikleri:**

Otomatik hesaplanan:
- Sınıf ortalaması
- Medyan
- Standart sapma
- En yüksek not
- En düşük not

**Not Dağılımı (Bar Chart):**
- Her harf notu için:
  - Öğrenci sayısı
  - Yüzde
  - Bar gösterimi

Başarı oranı hesaplanır.

İndirme seçenekleri:
- Grafik Görünümü
- PDF Rapor
- Excel İndir

**Toplu Not Yükleme:**

Adımlar:
1. Not şablonunu indir
2. Excel'e notları gir
3. Dosyayı yükle
4. Kontrol et ve onayla

**Not İtiraz Yönetimi:**

Liste:
- Bekleyen itirazlar
- Her itiraz için:
  - Öğrenci bilgisi
  - İtiraz açıklaması
  - Butonlar:
    - Görüntüle
    - Yanıtla
    - Notu Güncelle

---

#### 2.2.5 Öğrenci Yönetimi ve Danışmanlık

**Danışman Olduğum Öğrenciler:**

Tablo formatında:
- Sıra No
- Öğrenci No
- Ad Soyad
- Sınıf
- GANO
- Durum (Aktif/Onay Bekliyor)
- Detay butonu

Filtreler:
- Onay Bekleyenler
- Tüm Öğrenciler
- Sınıf bazlı

Raporlar:
- Akademik Durum Raporu
- Excel İndir

**Öğrenci Detay Profili:**

Bilgiler:
- Fotoğraf
- Ad Soyad
- Öğrenci numarası
- Bölüm ve sınıf
- İletişim bilgileri

Akademik Bilgiler:
- GANO ve YANO
- Toplam kredi
- Devamsızlık durumu

Bekleyen İşlemler:
- Ders seçimi onayı
- Mazeret raporları

Alınan Dersler (Bu Dönem):
- Ders listesi
- Notlar

Danışmanlık Notları:
- Not ekleme alanı
- Geçmiş notlar

İletişim Geçmişi:
- Görüşme tarihleri
- Görüşme konuları

Butonlar:
- Ders Seçimi Onayla
- Mesaj Gönder
- Randevu Ver

**Ders Seçimi Onay Ekranı:**

Öğrenci bilgileri

Seçilen Dersler:
- Her ders için:
  - Ders kodu ve adı
  - AKTS
  - Ön koşul kontrolü (✓/⚠️)

Toplam AKTS kontrolü

Uyarılar (varsa):
- Ön koşul eksiklikleri
- Kredi aşımı

Danışman Notu:
- Opsiyonel not ekleme

Butonlar:
- Tümünü Onayla
- Uyarılarla Onayla
- Red Et
- Öğrenciyle Görüş

**Danışmanlık Randevu Sistemi:**

Bugünkü Randevular:
- Saat
- Öğrenci
- Konu
- Butonlar:
  - Başlat
  - İptal
  - Ertele

Yarınki Randevular

Butonlar:
- Yeni Randevu Aç
- Randevu Ayarları
- Takvim Görünümü

---

#### 2.2.6 Yoklama Sistemi

**Yoklama Alma Ekranı:**

Ders bilgileri:
- Ders adı
- Tarih ve saat
- Hafta
- Konu

Yoklama Yöntemleri:
- **QR Kod ile Otomatik:**
  - QR kod göster
  - Aktif süre
  - Yoklama alındı: X/Y
  - Butonlar:
    - QR Kodu Göster
    - Süreyi Uzat
    - Manuel Tamamla

- **Manuel Yoklama:**
  - Öğrenci listesi
  - Her öğrenci için:
    - Var/Yok seçimi
    - Değiştir butonu
  - Butonlar:
    - Tümünü Var
    - Tümünü Yok
    - Kaydet

**Devamsızlık Raporu:**

Genel Bilgiler:
- Toplam ders saati
- Maksimum devamsızlık hakkı

Uyarı:
- Dikkat gerektiren öğrenci sayısı

Devamsızlık Dağılımı (Bar Chart):
- 0-10%: X öğrenci
- 10-20%: X öğrenci
- 20-30%: X öğrenci ⚠️
- >30%: X öğrenci (Düşecek)

Butonlar:
- Detaylı Rapor
- Uyarı Gönder
- Excel İndir

---

#### 2.2.7 Giriş-Çıkış Kayıtlarım

**Son 30 Gün Özeti:**
- Toplam çalışma günü
- Ortalama mesai süresi
- En erken giriş
- En geç çıkış
- Haftalık ders saati
- Kampüste geçirilen toplam süre

**Detaylı Kayıt Listesi:**

Tablo formatında:
- Tarih
- Saat
- Tür (Giriş/Çıkış)
- Lokasyon
- Cihaz

Filtreleme ve indirme

**Aylık Mesai Raporu:**
- Çalışma günü
- Toplam mesai
- Haftalık ortalama
- Ders saati
- Danışmanlık
- Araştırma

Butonlar:
- Detaylı Rapor
- PDF İndir

---

#### 2.2.8 Harcama ve Yükleme Kayıtlarım

**Mali Durum:**
- Güncel bakiye
- Son işlem
- Butonlar

**Harcama Geçmişi:**
- Tarih, Saat, Açıklama, Tutar, Bakiye

**Yükleme Geçmişi:**
- Tarih, Saat, Yöntem, Tutar, Durum

**Aylık Harcama Analizi:**
- Toplam harcama
- Günlük ortalama
- Kategori bazlı harcama
- Önceki ay karşılaştırması

---

#### 2.2.9 Sınav Yönetimi

**Yeni Sınav Oluştur:**

Form:
- Ders seçimi
- Sınav türü (Vize/Final/Quiz/Bütünleme)
- Tarih ve saat
- Salon
- Kapsam
- Değerlendirme ağırlığı

Butonlar:
- Sınav Oluştur
- İptal

---

#### 2.2.10 Araştırma ve Yayınlar

**Yayınlarım:**

Butonlar:
- Yeni Yayın Ekle
- İçe Aktar (BibTeX)
- Dışa Aktar

Yıl bazlı liste:
- Her yayın için:
  - Başlık
  - Dergi/Konferans
  - Etki faktörü
  - Detay, PDF, Atıf sayısı

Özet:
- Toplam atıf
- H-indeks

**Projelerim:**

Aktif Projeler:
- Her proje için:
  - Proje adı ve türü
  - Başlangıç ve bitiş tarihi
  - Bütçe (toplam ve harcanan)
  - İlerleme yüzdesi
  - Butonlar:
    - Detay
    - Rapor
    - Harcama

---

#### 2.2.11 Mesajlaşma

**Gelen Mesajlar:**

Filtreler:
- Tümü
- Öğrenciler
- Meslektaşlar
- İdari

Mesaj listesi:
- Gönderen
- Konu/Önizleme
- Tarih
- Okunmadı işareti

---

#### 2.2.12 Raporlama ve İstatistikler

**Ders İstatistiklerim:**
- Toplam öğrenci
- Ortalama başarı
- Ortalama devamsızlık

Ders Başarı Oranları (Bar Chart):
- Her ders için:
  - Başarı yüzdesi
  - AA oranı

Butonlar:
- Detaylı Rapor
- Karşılaştırmalı Analiz
- Excel İndir

**Danışmanlık İstatistikleri:**
- Toplam danışman öğrenci
- Ortalama GANO
- Aktif/Mezun sayıları
- Bu dönem görüşme sayısı
- Bekleyen onaylar

Butonlar:
- Detaylı Rapor
- Öğrenci Başarı Analizi

---

#### 2.2.13 Ayarlar ve Tercihler

**Bildirim Tercihleri:**

E-posta Bildirimleri:
- Öğrenci mesajları
- Not girişi hatırlatmaları
- Danışman onay talepleri
- Toplantı bildirimleri
- Günlük özet

SMS Bildirimleri:
- Acil toplantılar
- Son tarih uyarıları

Push Bildirimleri:
- Anlık mesajlar
- Sistem duyuruları

Kaydet butonu

---

### 2.3 İDARİ PERSONEL PORTALI

**Portal URL:** admin.university.edu

#### 2.3.1 Rol Bazlı Dashboard'lar

**İnsan Kaynakları Personeli:**
- Toplam personel
- İşe alım bekleyen
- İzin bekleyen
- Bordro durumu
- Bekleyen işler listesi

**Muhasebe Personeli:**
- Aylık gelir
- Bekleyen ödeme
- Bütçe kullanım
- Kasa bakiye
- Bekleyen işler listesi

**Satın Alma Personeli:**
- Talep bekliyen
- Aktif ihale
- Tedarikçi sayısı
- Bekleyen teslimat
- Bekleyen işler listesi

**Güvenlik Personeli:**
- Kampüste kişi sayısı
- Ziyaretçi kayıt
- Olay kaydı
- Aktif kamera
- Güncel durum

---

### 2.4 REKTÖRLÜK / YÖNETİM PORTALI

**Portal URL:** executive.university.edu

**Yönetici Kontrol Paneli:**

Büyük kartlar:
- Toplam öğrenci (trend)
- Akademik personel sayısı
- İdari personel sayısı
- Mali durum (bütçe kullanımı, gelir)

Akademik Başarı Trendi (Line Chart):
- Son 3-5 yılın GANO ortalaması

Fakülte Bazlı Başarı Oranları (Bar Chart):
- Her fakülte için başarı yüzdesi

Dikkat Gerektiren Konular:
- Kapasite sorunları
- Personel açıkları
- Altyapı ihtiyaçları

Butonlar:
- Detaylı Raporlar
- Stratejik Plan
- Bütçe Analizi

---

## 3. MODÜLLER VE ALT SİSTEMLER

### MODÜL 1: AKADEMİK YÖNETİM SİSTEMİ

#### 1.1 Öğrenci İşleri Modülü

**Öğrenci Kayıt ve Kabul:**
- Online başvuru sistemi
- Belge doğrulama
- Kayıt işlemleri
- Öğrenci numarası otomatik ataması
- Yatay/Dikey geçiş işlemleri

**Öğrenci Bilgileri Yönetimi:**
- Kişisel bilgiler
- İletişim bilgileri
- Acil durum kişileri
- Öğrenci fotoğrafı
- Sağlık bilgileri
- Engel durumu

**Öğrenci Statü Yönetimi:**
- Aktif / Pasif / Mezun / Kayıt Dondurma
- Disiplin kayıtları
- Uyarı sistemi

#### 1.2 Ders ve Müfredat Yönetimi

**Müfredat Planlama:**
- Bölüm müfredatları
- Ders havuzu
- Zorunlu/Seçmeli dersler
- Ön koşul tanımlama
- AKTS kredileri

**Ders Açma ve Programlama:**
- Dönemlik ders açma
- Kontenjan belirleme
- Ders grubu oluşturma
- Sınıf ve zaman atama
- Öğretim üyesi atama

**Ders Kayıt Sistemi:**
- Online ders seçimi
- Danışman onayı
- Çakışma kontrolü
- Ders ekleme/bırakma
- Kayıt tarihleri yönetimi

#### 1.3 Sınav ve Değerlendirme Sistemi

**Sınav Yönetimi:**
- Sınav programı oluşturma
- Salon ataması
- Gözetmen ataması
- Mazeret sınavları
- Bütünleme sınavları

**Not Yönetimi:**
- Not girişi (öğretim üyesi tarafından)
- Vize/Final oranları
- Harf notu dönüşümü
- Ortalama hesaplama (GANO/YANO)
- Not itiraz sistemi
- Transkript oluşturma

**Değerlendirme Kriterleri:**
- Ödev değerlendirme
- Proje değerlendirme
- Quiz/Kısa sınav
- Laboratuvar değerlendirme
- Sunum değerlendirme

#### 1.4 Mezuniyet İşlemleri

- Mezuniyet koşulları kontrolü
- Diploma hazırlama
- Diploma takip sistemi
- Apostil işlemleri
- Mezun bilgi sistemi

#### 1.5 Danışmanlık Sistemi

- Öğrenci-danışman eşleştirme
- Danışmanlık görüşme kayıtları
- Akademik danışmanlık raporları
- Ders seçim onayları

#### 1.6 Devamsızlık Takip Sistemi

- Derslere yoklama (QR kod / Manuel)
- Devamsızlık oranı hesaplama
- Otomatik uyarı sistemi
- Mazeret belge yönetimi

---

### MODÜL 2: AKADEMİK PERSONEL YÖNETİMİ

#### 2.1 Öğretim Üyesi Yönetimi

- Öğretim üyesi sicil bilgileri
- Akademik unvan takibi
- Ders yükü hesaplama
- Yayın ve proje takibi
- Performans değerlendirme

#### 2.2 Ders ve Sınav Yönetimi

- Ders planı oluşturma
- Syllabus yönetimi
- Dönem içi ödev/proje atama
- Not girişi
- Öğrenci danışmanlığı

#### 2.3 Araştırma ve Yayın Yönetimi

- Yayın kayıt sistemi
- Atıf takibi
- Araştırma projeleri
- Patent ve telif hakları
- Akademik özgeçmiş

---

### MODÜL 3: İNSAN KAYNAKLARI VE PERSONEL YÖNETİMİ

#### 3.1 Personel Kayıt ve Sicil

- Personel bilgi yönetimi
- Sözleşme bilgileri
- Pozisyon tanımları
- Kadro yönetimi
- SGK bildirimleri

#### 3.2 İşe Alım Süreci

- İlan yönetimi
- Başvuru toplama
- Mülakat planlama
- Değerlendirme formu
- İşe alım onayları

#### 3.3 İzin Yönetimi

- Yıllık izin hesaplama
- İzin talep ve onay
- Mazeret izni
- Raporlu izin
- Ücretsiz izin
- İzin bakiye takibi
- Toplu izin planlaması

#### 3.4 Mesai ve Vardiya Yönetimi

- Vardiya tanımlama
- Mesai saatleri
- Fazla mesai hesaplama
- Esnek çalışma
- Uzaktan çalışma takibi

#### 3.5 Performans Değerlendirme

- Değerlendirme kriterleri
- Periyodik değerlendirmeler
- 360 derece değerlendirme
- Hedef belirleme
- KPI takibi

#### 3.6 Eğitim ve Gelişim

- Eğitim ihtiyaç analizi
- Eğitim planlaması
- Eğitim takibi
- Sertifika yönetimi

---

### MODÜL 4: BORDRO VE ÜCRET YÖNETİMİ

#### 4.1 Maaş Hesaplama (Türkiye Mevzuatına Uygun)

**Akademik Personel Bordrosu:**
- Derece/Kademe sistemi
- Ek göstergeler
- Akademik teşvik
- Ders ücreti hesaplama
- Tazminatlar

**İdari Personel Bordrosu:**
- Kadro/Pozisyon maaşı
- Kıdem tazminatı
- Performans primi
- Ek ödemeler

**Kesintiler:**
- Gelir vergisi hesaplama
- SGK primi kesintisi (işçi + işveren)
- Sendika aidatı
- İcra kesintileri
- Avans kesintileri

#### 4.2 Ek Ödeme Yönetimi

- Fazla mesai ödemeleri
- Nöbet ücreti
- İkramiye hesaplama
- Döner sermaye payları

#### 4.3 Bordro Raporlama

- Aylık bordro listesi
- Muhtasar beyanname
- SGK bildirge
- Banka ödeme dosyası (XML)
- Vergi dairesi raporları

---

### MODÜL 5: MALİ İŞLER VE MUHASEBE

#### 5.1 Öğrenci Mali İşler

**Harç ve Ücret Yönetimi:**
- Dönemlik harç
- Ders başına ücret
- Yaz okulu ücretleri
- İntibak ücretleri
- Belge ücretleri

**Ödeme Sistemi:**
- Virtual POS entegrasyonu (Get724, NestPay, Iyzico, PayTR)
- Havale/EFT takibi
- Kredi kartı taksitlendirme
- Burs indirimi uygulama
- Ödeme planı oluşturma

**Borç Takip:**
- Otomatik borç bildirimi
- Ödeme hatırlatıcıları
- Borçlu öğrenci raporu
- Kayıt dondurma

#### 5.2 Genel Muhasebe

**Genel Muhasebe Sistemi:**
- Hesap planı
- Yevmiye kayıtları
- Defteri kebir
- Mizan
- Bilanço
- Gelir-gider tablosu

**Cari Hesap Yönetimi:**
- Tedarikçi carisi
- Personel carisi
- Öğrenci carisi
- Banka hesapları

**Kasa Yönetimi:**
- Nakit giriş/çıkış
- Kasa sayımı
- Kasa devir işlemleri

#### 5.3 Bütçe ve Planlama

- Yıllık bütçe hazırlama
- Bütçe dağılımı (birim bazlı)
- Bütçe takibi
- Harcama onayları
- Bütçe revizyonu

#### 5.4 Döner Sermaye İşletmesi

- Döner sermaye gelirleri
- Personel payları hesaplama
- Döner sermaye muhasebesi
- Aylık/Yıllık raporlar

---

### MODÜL 6: SATIN ALMA VE TEDARİK ZİNCİRİ

#### 6.1 Satın Alma Yönetimi

**Talep Yönetimi:**
- Birimlerden talep toplama
- Talep onay süreci
- Acil talep işlemleri

**Tedarikçi Yönetimi:**
- Tedarikçi kayıt sistemi
- Tedarikçi değerlendirme
- Tedarikçi sözleşmeleri
- Kara liste yönetimi

**Satın Alma Süreci:**
- Teklif toplama
- Karşılaştırma tablosu
- Sipariş oluşturma
- Sipariş takibi
- Teslimat kontrolü

#### 6.2 İhale Yönetimi

**İhale Türleri:**
- Açık ihale
- Belli istekliler arası ihale
- Pazarlık usulü
- Doğrudan temin

**İhale Süreci:**
- İhale dokümanı hazırlama
- İlan yayınlama
- Teklif alma
- İhale komisyonu değerlendirmesi
- Sonuç bildirimi
- İtiraz süreci
- Sözleşme imzalama

#### 6.3 Stok Yönetimi (Kampüs Bazlı)

**Envanter Yönetimi:**
- Malzeme kayıt sistemi
- Stok kartları
- Seri/Lot takibi
- Minimum stok uyarısı
- FIFO/LIFO hesaplama

**Depo Yönetimi (Her kampüs için ayrı):**
- Depo tanımları
- Raf/Lokasyon yönetimi
- Giriş/Çıkış işlemleri
- Depo transfer
- Sayım işlemleri

**Malzeme Grupları:**
- Ofis malzemeleri (Kalem, defter, kağıt, kartuş...)
- Temizlik malzemeleri
- Laboratuvar malzemeleri
- Bilgisayar ve elektronik
- Mobilya
- Teknik ekipman
- Gıda maddeleri

#### 6.4 Demirbaş Takip

- Demirbaş kayıt
- Zimmet işlemleri
- Transfer işlemleri
- Amortisman hesaplama
- Sayım ve envanter

---

### MODÜL 7: YEMEKHANE VE KAFETERİA YÖNETİMİ

#### 7.1 Yemekhane Sistemi

**Menü Yönetimi:**
- Günlük menü planlama
- Haftalık/Aylık menü
- Diyet menü seçenekleri
- Kalori hesaplama

**Yemek Kartı/Abonelik Sistemi:**
- Öğrenci yemek kartı
- Personel yemek kartı
- Yükleme işlemleri (QR kod ile)
- Günlük yemek hakkı
- Misafir yemek satışı

**Rezervasyon Sistemi:**
- Online rezervasyon
- Öğün bazlı rezervasyon
- İptal işlemleri

**Yemekhane Operasyon:**
- Yemek çıkış raporu
- Porsiyon kontrolü
- Fire/İsraf takibi
- Temizlik kontrol

#### 7.2 Kantin/Kafeterya Yönetimi

**Ürün Yönetimi:**
- Ürün tanımlama
- Fiyat belirleme
- Stok takibi
- Kategori yönetimi

**Satış Sistemi (POS):**
- Kart ile ödeme (öğrenci/personel kartı)
- Nakit ödeme
- Kredi kartı
- Kampanyalar

**Raporlama:**
- Günlük satış raporu
- Ürün bazlı satış
- Karlılık analizi
- En çok satan ürünler

#### 7.3 Gıda Güvenliği ve Hijyen

- HACCP kayıtları
- Gıda kontrol formları
- Sağlık muayene takibi (personel)
- Dezenfeksiyon kayıtları

---

### MODÜL 8: KÜTÜPHANE YÖNETİM SİSTEMİ

#### 8.1 Materyal Yönetimi

**Kitap Yönetimi:**
- Kitap kayıt (ISBN, yazar, yayınevi, baskı...)
- Çoklu kopya yönetimi
- Kitap kategorileri
- Dijital kitaplar (e-book)

**Dergi ve Süreli Yayınlar:**
- Dergi abonelik takibi
- Sayı bazlı kayıt

**Tez Arşivi:**
- Lisans/Yüksek Lisans/Doktora tezleri
- Tez erişim yetkileri

**Diğer Materyaller:**
- DVD/CD
- Harita
- Görsel-işitsel materyaller

#### 8.2 Ödünç Verme Sistemi

**Ödünç İşlemleri:**
- Kitap ödünç alma (QR kod ile)
- Süre hesaplama (öğrenci/öğretim üyesi için farklı)
- Yenileme (renewal)
- Rezervasyon

**İade İşlemleri:**
- İade kontrolü
- Gecikmeli iade ceza hesaplama
- Hasar değerlendirme

**Ödünç Kuralları:**
- Kullanıcı tipine göre limit
- Ödünç süresi
- Ceza miktarları

#### 8.3 Kütüphane Kullanım

- Online katalog (OPAC)
- Gelişmiş arama
- Kitap rezervasyonu
- Okuyucu koltuğu rezervasyonu
- Grup çalışma odası rezervasyonu

#### 8.4 Kütüphane Raporlama

- Popüler kitaplar
- Ödünç istatistikleri
- Borçlu kullanıcılar
- Envanter raporu

---

### MODÜL 9: GÜVENLİK VE ERİŞİM KONTROL SİSTEMİ

#### 9.1 Turnike ve Kapı Kontrol Sistemi

**Giriş-Çıkış Yönetimi:**
- Kart okuyucu entegrasyonu
- QR kod okuma
- Biyometrik entegrasyon (parmak izi, yüz tanıma)
- RFID kart sistemi

**Erişim Yönetimi:**
- Bina bazlı yetkilendirme
- Zaman bazlı erişim (mesai saatleri)
- Özel izin gerektiren alanlar
- Geçici yetki verme (misafir)

**Ziyaretçi Yönetimi:**
- Ziyaretçi kayıt
- Misafir kartı
- Eskort takibi
- Ziyaret süresi kontrolü

#### 9.2 Kamera İzleme Sistemi (CCTV)

**Kamera Yönetimi:**
- Kamera kayıt ve konumlandırma
- Canlı görüntü izleme
- Kayıt saklama süreleri
- Kamera arıza bildirimi

**Olay Yönetimi:**
- Olay kaydı
- Video analizi
- Yüz tanıma entegrasyonu (AI)
- Şüpheli davranış tespiti

**Yetkilendirme:**
- Kamera erişim yetkileri
- Kayıt inceleme yetkileri
- Log kayıtları

#### 9.3 Acil Durum Yönetimi

**İçerde/Dışarıda Kişi Takibi:**
- Anlık lokasyon takibi (turnike geçişleri)
- Kampüste bulunan kişi sayısı
- Bina bazlı dağılım
- Acil durum sinyali

**Acil Durum Senaryoları:**
- Yangın
- Deprem
- Güvenlik tehdidi
- Sağlık acili
- Doğal afet

**Tahliye Yönetimi:**
- Tahliye planları
- Toplanma noktaları
- Yoklama sistemi
- Eksik kişi tespiti

**Bildirim Sistemi:**
- SMS bildirimi
- E-posta bildirimi
- Push notification
- Sesli anons sistemi entegrasyonu

#### 9.4 Güvenlik Personeli Yönetimi

- Nöbet çizelgesi
- Devriye rotaları
- Olay raporlama
- İletişim sistemi

---

### MODÜL 10: PARK YÖNETİM SİSTEMİ

#### 10.1 Araç Kayıt

- Araç bilgileri (plaka, marka, model, renk)
- Kullanıcı eşleştirme
- Geçici araç kaydı

#### 10.2 Park Yeri Yönetimi

- Otopark tanımlama
- Park yeri atama
- Rezervasyon sistemi
- Misafir park yeri

#### 10.3 Giriş-Çıkış Kontrolü

- Plaka tanıma sistemi (ANPR)
- Bariyer kontrolü
- Otopark doluluk oranı
- Park süresi hesaplama

#### 10.4 Ücretlendirme

- Ücretli/Ücretsiz alanlar
- Saatlik ücretlendirme
- Abonelik sistemi
- Ödeme entegrasyonu

---

### MODÜL 11: SAĞLIK HİZMETLERİ

#### 11.1 Öğrenci Sağlık Merkezi

- Sağlık kayıtları
- Muayene randevusu
- Reçete yönetimi
- Aşı takibi
- Kronik hastalık kayıtları

#### 11.2 Sağlık Raporları

- Rapor takibi
- Mazeret raporu onayı
- Uzun süreli raporlar

#### 11.3 İş Sağlığı ve Güvenliği

- Periyodik sağlık kontrolleri
- İş kazası kayıtları
- Risk değerlendirme
- OSGB entegrasyonu

---

### MODÜL 12: LABORATUVAR YÖNETİMİ

#### 12.1 Laboratuvar Kaynakları

- Laboratuvar tanımları
- Ekipman kayıt
- Malzeme stok yönetimi
- Kimyasal madde takibi (MSDS)

#### 12.2 Laboratuvar Rezervasyonu

- Ders için rezervasyon
- Araştırma için rezervasyon
- Zaman dilimi yönetimi
- Ekipman rezervasyonu

#### 12.3 Güvenlik ve Kalibrasyon

- Ekipman kalibrasyon takibi
- Güvenlik kontrol formları
- Atık yönetimi
- Tehlikeli madde envanter

---

### MODÜL 13: ARAŞTIRMA PROJELERİ YÖNETİMİ

#### 13.1 Proje Başvuru ve Onay

- Proje başvuru formu
- Bütçe planlaması
- Değerlendirme süreci
- Onay/Red süreçleri

#### 13.2 Proje Takip

- İlerleme raporları
- Harcama takibi
- Çıktı yönetimi (yayın, patent...)
- Proje ekibi yönetimi

#### 13.3 Dış Fonlu Projeler

- TÜBİTAK projeleri
- AB projeleri
- Sanayi işbirlikleri
- Uluslararası projeler

---

### MODÜL 14: ÖĞRENCİ İŞLERİ VE AKTİVİTELER

#### 14.1 Öğrenci Kulüpleri

- Kulüp kayıt sistemi
- Üye yönetimi
- Etkinlik planlaması
- Bütçe takibi

#### 14.2 Sosyal Etkinlikler

- Etkinlik oluşturma
- Katılımcı kaydı
- Mekan rezervasyonu
- Katılım sertifikası

#### 14.3 Burs ve Yardımlar

- Burs başvuruları
- Değerlendirme süreci
- Ödeme planlaması
- Sosyal yardım

---

### MODÜL 15: İLETİŞİM VE BİLGİLENDİRME

#### 15.1 Duyuru Sistemi

- Genel duyurular
- Hedef kitle seçimi
- Acil duyurular
- Duyuru arşivi

#### 15.2 Mesajlaşma Sistemi

- Birim içi mesajlaşma
- Toplu mesaj gönderimi
- E-posta entegrasyonu
- SMS entegrasyonu

#### 15.3 Anket ve Geri Bildirim

- Anket oluşturma
- Online yanıt toplama
- Sonuç analizi
- Raporlama

---

### MODÜL 16: BELGE YÖNETİMİ

#### 16.1 Öğrenci Belgeleri

- Öğrenci belgesi
- Transkript
- Diploma
- Onay belgeleri
- E-imza entegrasyonu

#### 16.2 Resmi Yazışmalar

- Evrak kayıt sistemi (EBYS benzeri)
- Gelen/Giden evrak
- İmza yönetimi
- Arşivleme

#### 16.3 Dijital Arşiv

- Dosya yükleme
- Kategorizasyon
- Arama ve filtreleme
- Erişim yetkileri

---

### MODÜL 17: TEKNİK HİZMETLER VE BAKIM

#### 17.1 Arıza ve Talep Yönetimi

- Arıza bildirimi (ticket system)
- Önceliklendirme
- Teknisyen atama
- İş takibi
- Tamamlanma kontrolü

#### 17.2 Periyodik Bakım

- Bakım planları
- Bakım takvimi
- Bakım kayıtları
- Ekipman ömrü takibi

#### 17.3 Enerji Yönetimi

- Elektrik tüketim takibi
- Su tüketimi
- Doğalgaz tüketimi
- Enerji optimizasyonu

---

### MODÜL 18: BİLGİ TEKNOLOJİLERİ YÖNETİMİ

#### 18.1 IT Altyapı Yönetimi

- Sunucu yönetimi
- Network yönetimi
- Lisans yönetimi
- Yedekleme sistemi

#### 18.2 Kullanıcı Hesap Yönetimi

- Kullanıcı oluşturma
- Şifre politikaları
- Yetki matrisi
- Single Sign-On (SSO)

#### 18.3 Helpdesk

- Destek talebi
- Uzaktan destek
- Bilgi bankası
- Sık sorulan sorular

---

## 4. TEKNİK MİMARİ

### 4.1 Clean Architecture Katmanları

**Presentation Layer:**
- Web API Controllers
- SignalR Hubs (real-time communication)
- Background Jobs (Hangfire)

**Application Layer:**
- CQRS (Commands & Queries)
- MediatR Handlers
- DTOs & ViewModels
- FluentValidation
- AutoMapper Profiles

**Domain Layer:**
- Entities & Aggregates
- Value Objects
- Domain Events
- Domain Services
- Repository Interfaces
- Specifications

**Infrastructure Layer:**
- EF Core DbContext
- Repository Implementations
- External API Integrations
- File Storage (Azure/AWS/MinIO)
- Email/SMS Services
- Caching (Redis)
- Message Queue (RabbitMQ)

### 4.2 SOLID Prensipleri Uygulaması

**Single Responsibility (SRP):**
Her service tek bir sorumluluğa sahiptir.

**Open/Closed (OCP):**
Extension ile genişletilebilir yapı.

**Liskov Substitution (LSP):**
Alt sınıflar üst sınıf yerine kullanılabilir.

**Interface Segregation (ISP):**
Küçük, özel arayüzler kullanılır.

**Dependency Inversion (DIP):**
Bağımlılıklar abstraction'lara yapılır.

### 4.3 Teknoloji Stack

**Backend:**
- .NET 9.0
- ASP.NET Core Web API
- Entity Framework Core 9.0
- SQL Server 2022
- MediatR (CQRS pattern)
- FluentValidation
- AutoMapper
- Serilog (Structured logging)
- Hangfire (Background jobs)
- SignalR (Real-time communication)
- Redis (Distributed caching)
- RabbitMQ (Message queue)

**Authentication & Authorization:**
- JWT Bearer Tokens
- IdentityServer4 / Keycloak
- OAuth 2.0 / OpenID Connect
- Role-based access control (RBAC)
- Permission-based access control

**API Documentation:**
- Swagger/OpenAPI 3.0
- ReDoc

**Testing:**
- xUnit
- Moq
- FluentAssertions
- Testcontainers

**DevOps:**
- Docker & Docker Compose
- Kubernetes
- GitHub Actions / Azure DevOps
- SonarQube (Code quality)
- OWASP Dependency Check

---

## 5. VERİTABANI YAPISI

### 5.1 Aggregate Root'lar (DDD)

**Person Aggregate:**
- Person (Root)
- Student
- Staff
- Address
- EmergencyContact

**Academic Aggregate:**
- Faculty (Root)
- Department
- Course
- Curriculum
- Prerequisite

**Enrollment Aggregate:**
- Enrollment (Root)
- CourseRegistration
- Grade
- Attendance
- ExamResult

**Financial Aggregate:**
- Payment (Root)
- Invoice
- Transaction
- Installment
- Fee

**HR Aggregate:**
- Employee (Root)
- Contract
- Leave
- Shift
- Payroll

**Procurement Aggregate:**
- PurchaseRequest (Root)
- PurchaseOrder
- Supplier
- Tender
- TenderBid

**Inventory Aggregate (per Campus):**
- Warehouse (Root)
- Stock
- StockMovement
- Item
- StockCount

**Library Aggregate:**
- Material (Root)
- Loan
- Reservation
- Fine
- Category

**Security Aggregate:**
- AccessPoint (Root)
- AccessLog
- EmergencyAlert
- Visitor
- Camera

**Facility Aggregate:**
- Building (Root)
- Room
- Laboratory
- ParkingLot
- Equipment

### 5.2 Ortak Tablolar

- Users
- Roles
- UserRoles
- Permissions
- AuditLogs
- Notifications
- Documents
- Attachments
- Settings
- SystemLogs

---

## 6. API ENDPOINT'LERİ

### 6.1 Endpoint Grupları

```
/api/v1/auth/*               - Authentication & Authorization
/api/v1/students/*           - Student operations
/api/v1/staff/*              - Staff operations
/api/v1/courses/*            - Course management
/api/v1/enrollments/*        - Enrollment operations
/api/v1/grades/*             - Grade management
/api/v1/attendance/*         - Attendance tracking
/api/v1/exams/*              - Exam management
/api/v1/payments/*           - Payment operations
/api/v1/transactions/*       - Transaction history
/api/v1/hr/*                 - HR operations
/api/v1/leaves/*             - Leave management
/api/v1/payroll/*            - Payroll operations
/api/v1/procurement/*        - Procurement operations
/api/v1/tenders/*            - Tender management
/api/v1/inventory/*          - Inventory management (per campus)
/api/v1/library/*            - Library operations
/api/v1/security/*           - Security & access control
/api/v1/access-logs/*        - Access log tracking
/api/v1/emergency/*          - Emergency management
/api/v1/facilities/*         - Facility management
/api/v1/cafeteria/*          - Cafeteria operations
/api/v1/parking/*            - Parking management
/api/v1/health/*             - Health services
/api/v1/laboratory/*         - Laboratory management
/api/v1/research/*           - Research projects
/api/v1/clubs/*              - Student clubs
/api/v1/events/*             - Event management
/api/v1/documents/*          - Document management
/api/v1/announcements/*      - Announcement system
/api/v1/messages/*           - Messaging system
/api/v1/reports/*            - Reporting
/api/v1/analytics/*          - Analytics & dashboards
/api/v1/notifications/*      - Notification management
/api/v1/settings/*           - System settings
/api/v1/admin/*              - Admin operations
```

---

## 7. GÜVENLİK VE YETKİLENDİRME

### 7.1 Güvenlik Özellikleri

- JWT Token-based authentication
- Refresh token rotation
- Role-based access control (RBAC)
- Permission-based access control
- Data encryption (at rest & in transit)
- SQL Injection protection (parameterized queries)
- XSS protection
- CSRF protection
- Rate limiting
- IP whitelist/blacklist
- Two-factor authentication (2FA)
- Strong password policies
- Comprehensive audit logging
- GDPR compliance
- KVKK uyumlu (Türkiye için)

### 7.2 Yetki Matrisi (Örnek)

| Modül | Süper Admin | Rektör | Dekan | Öğretim Üyesi | Öğrenci | Personel |
|-------|------------|--------|-------|--------------|---------|----------|
| Kullanıcı Yönetimi | CRUD | R | R | - | - | - |
| Öğrenci Kayıt | CRUD | R | RU | R | R (Own) | R |
| Not Girişi | R | R | R | CRU | R (Own) | - |
| Maaş Bordrosu | CRUD | R | - | R (Own) | - | R (Own) |
| İhale Yönetimi | CRUD | RU | R | - | - | R |

*C: Create, R: Read, U: Update, D: Delete*

---

## 8. ENTEGRASYONLAR

### 8.1 Dış Sistem Entegrasyonları

**Ödeme Sistemleri:**
- Virtual POS (Get724, NestPay, Iyzico, PayTR)
- Havale/EFT bildirimleri (banka API)

**Devlet Kurumları:**
- e-Devlet entegrasyonu
- MEB entegrasyonu
- YÖK entegrasyonu
- SGK entegrasyonu
- GİB entegrasyonu

**Bildirim Kanalları:**
- SMS (Netgsm, İleti Merkezi)
- E-posta (SMTP, SendGrid)
- Push notification (Firebase)

**Donanım Entegrasyonları:**
- Turnike sistemleri (RFID, QR reader)
- Kart okuyucular
- Biyometrik cihazlar
- Kameralar (ONVIF protocol)
- Plaka okuma (ANPR)
- POS cihazları

**Dosya Depolama:**
- Azure Blob Storage / AWS S3
- MinIO (self-hosted)

**Dış Servisler:**
- Google Maps API (lokasyon)
- E-imza servisleri
- OCR servisleri

---

## 9. DEPLOYMENT STRATEJİSİ

### 9.1 Deployment Mimarisi

**Load Balancer:**
Nginx / HAProxy

**API Nodes:**
- Multiple containerized instances
- Auto-scaling

**Database:**
- SQL Server Always On (High Availability)
- Master-Slave replication

**Caching:**
- Redis Cluster

**Message Queue:**
- RabbitMQ Cluster

### 9.2 Scalability

- **Horizontal scaling:** Load balancer arkasında multiple API instances
- **Database replication:** Master-slave setup
- **Caching:** Redis for frequently accessed data
- **Message queue:** Async operations için RabbitMQ
- **CDN:** Static assets için CloudFlare
- **Microservices ready:** Her modül ayrı service'e dönüştürülebilir

---

## 10. RAPORLAMA VE ANALİTİK

### 10.1 Raporlar

**Akademik Raporlar:**
- Öğrenci başarı istatistikleri
- Ders başarı oranları
- Devamsızlık raporları
- Mezuniyet tahminleri
- Akademik takvim raporları

**Mali Raporlar:**
- Gelir-gider tablosu
- Nakit akışı
- Borç-alacak durumu
- Bütçe gerçekleşme oranı
- Departman bazlı maliyet analizi

**İnsan Kaynakları Raporları:**
- Personel dağılım raporları
- İzin kullanım istatistikleri
- Bordro özet raporları
- Devir hızı (turnover rate)

**Operasyonel Raporlar:**
- Kütüphane kullanım istatistikleri
- Yemekhane tüketim raporları
- Enerji tüketim raporları
- Arıza/Talep istatistikleri

### 10.2 Dashboard'lar

- Rektörlük dashboard (executive summary)
- Dekan dashboard
- Öğrenci dashboard
- Öğretim üyesi dashboard
- Mali işler dashboard
- İnsan kaynakları dashboard

---

## 11. PERFORMANS VE OPTİMİZASYON

- Database indexing strategy
- Query optimization (EF Core)
- Caching (Redis)
- Lazy loading vs Eager loading
- Pagination (her liste için)
- Response compression
- CDN usage
- Background jobs (Hangfire)
- Async/await best practices

---

## PROJE KAPSAMI ÖZETİ

**Toplam Modül Sayısı:** 18 Ana Modül

**Toplam Alt Sistem:** 100+ Alt Sistem

**Toplam Entity:** 300+ Entity

**Toplam API Endpoint:** 500+ Endpoint

**Toplam Rol:** 15+ Farklı Kullanıcı Rolü

---

## SONRAKI ADIMLAR

Bu taslak onaylandıktan sonra, aşağıdaki fazlarda kod geliştirme yapılacaktır:

### Faz 1: Core Infrastructure (Temel Altyapı)
- Clean Architecture kurulumu
- CQRS pattern implementasyonu
- Domain entities ve value objects
- Repository ve Unit of Work pattern
- Authentication & Authorization

### Faz 2: Akademik Modüller
- Öğrenci yönetimi
- Ders yönetimi
- Not sistemi
- Devamsızlık takibi

### Faz 3: İdari Modüller
- İnsan kaynakları
- Bordro sistemi
- Mali işler
- Satın alma ve stok

### Faz 4: Operasyonel Modüller
- Güvenlik ve erişim kontrol
- Kütüphane
- Yemekhane/Kafeterya
- Laboratuvar

### Faz 5: Destek Modüller
- Belge yönetimi
- Raporlama
- Bildirim sistemi
- IT yönetimi

---

**Doküman Sonu**

*Hazırlayan: Yakup EYİSAN*  
*Tarih: 7 Ekim 2025*  
*Versiyon: 1.0*



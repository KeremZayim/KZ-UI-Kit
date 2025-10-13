# KZ-UI-Kit

Modern .NET uygulamaları için özel olarak geliştirilmiş **KZ-UI-Kit**, şık, yeniden kullanılabilir ve performans odaklı kullanıcı arayüzü (UI) bileşenleri sunar. Bu kütüphane, geliştiricilerin arayüz tasarım sürecini hızlandırmak, kod tekrarını azaltmak ve tutarlı bir tasarım deneyimi sağlamak amacıyla hazırlanmıştır.  

---

## 🎯 Özellikler

- Modern ve minimalist tasarımlar için optimize edilmiş bileşenler  
- Kolay entegrasyon ve kullanım  
- Özelleştirilebilir renk, boyut ve davranış seçenekleri  
- Performans dostu ve hafif yapı  
- Tekrar kullanılabilir kontrollerle kodunuzu sadeleştirir  

---

## 📦 Kontroller (UI Bileşenleri)

KZ-UI-Kit, aşağıdaki kontrolleri içerir ve `KZ-CustumUIKit/Controls` klasöründe bulunur:

| Komponent Adı | Açıklama |
|----------------|-----------|
| `KZ_CheckBox` | Özel tasarımlı onay kutusu bileşeni. Seçim veya durum belirtmek için kullanılır. |
| `KZ_DragControl` | Herhangi bir form veya panelin fare ile sürüklenebilmesini sağlar. |
| `KZ_Ellipse` | Form veya kontrol köşelerini ovalleştiren yardımcı bileşen. |
| `KZ_GradientPanel` | Arka planında renk geçişi (gradient) bulunan panel bileşeni. |
| `KZ_ListBox` | Liste öğelerini görüntülemek ve seçim yapmak için özelleştirilmiş ListBox. |
| `KZ_ListView` | Detaylı liste görünümü sunan, özelleştirilebilir ListView. |
| `KZ_ProgressBar` | Görevlerin ilerleme durumunu göstermek için şık bir ilerleme çubuğu. |
| `KZ_RadioButton` | Seçim gruplarında tek bir seçeneği aktif tutmak için özel RadioButton. |
| `KZ_TextBox` | Modern arayüzlü metin girişi kontrolü. Odak ve tema renkleri destekler. |
| `KZ_ToggleSwitch` | Açma/kapama (on/off) durumlarını göstermek için anahtar tipi kontrol. |
| `KZ_TrackBar` | Sayısal değerleri sürükleme hareketiyle değiştirmek için özelleştirilmiş TrackBar. |
| `KZ_UserCard` | Kullanıcı bilgilerini veya profil görsellerini göstermek için kart bileşeni. |

---

## 🛠️ Kurulum

1. KZ-UI-Kit kütüphanesini indirin veya NuGet üzerinden ekleyin.  
2. Projenizde `KZ_CustumUIKit.Controls` namespace’ini kullanın.  
3. Kontrolleri form veya panele ekleyerek kullanmaya başlayın.  

```csharp
using KZ_CustumUIKit.Controls;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();

        // Örnek kullanım
        var toggle = new KZ_ToggleSwitch();
        toggle.Checked = true;
        toggle.Location = new Point(50, 50);
        Controls.Add(toggle);

        var gradientPanel = new KZ_GradientPanel();
        gradientPanel.Size = new Size(200, 100);
        gradientPanel.GradientTopColor = Color.CornflowerBlue;
        gradientPanel.GradientBottomColor = Color.LightBlue;
        gradientPanel.Location = new Point(50, 100);
        Controls.Add(gradientPanel);
    }
}
```
## 💡 Önerilen Kullanım Senaryoları

Modern masaüstü uygulamalarında şık kontroller ile kullanıcı deneyimini artırmak

Tekrar kullanılabilir UI bileşenleri ile tutarlı bir tasarım sağlamak

Dashboard, yönetim panelleri veya profil ekranlarında görsel zenginlik eklemek

## 📖 Dokümantasyon

*Tamamlanınca eklenecek.*

## ⭐🛎️ Destek & İletişim ⭐

Herhangi bir soru, sorun veya öneri için kesinlikle destek talebinde bulunmanızı öneririz!

GitHub Issues üzerinden bildiriminizi oluşturabilirsiniz: Issues

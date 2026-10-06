# Hs.LangSense

**Multilingual value intelligence for .NET** — convert, normalize, validate and extract text and structured values across **Persian**, **Arabic**, **English**, **Kurdish Sorani** and **Kurdish Kurmanji**.


---

## What is Hs.LangSense?

Hs.LangSense is a practical .NET library for real-world multilingual data — especially Middle Eastern scripts and Iranian business identifiers.

It helps you:

- Convert **digits and letters** between language systems (`١٢٣` ↔ `۱۲۳` ↔ `123`)
- **Normalize** dirty user input (`ي` → `ی`, `ك` → `ک`, remove invisible characters)
- Fix **wrong keyboard layout** typing (`jsj` → `تست`)
- Turn **numbers into words** and back (Persian, English, Sorani, Kurmanji)
- **Validate** Iranian mobile numbers, national codes, bank cards and IBAN
- **Extract** phones, emails, cards and money amounts from free text
- **Mask** sensitive data in logs and UI
- Compare values **semantically** (`علي` equals `علی`)

Built for APIs, forms, OCR cleanup, search indexes, banking UIs and any system that receives mixed-language text.

---

## Install

```bash
dotnet add package Hs.LangSense
```


```csharp
using Hs.LangSense;
```

---

## When to use it

| Scenario | How LangSense helps |
|----------|---------------------|
| User typed with the wrong keyboard | `FixPersianKeyboard()` recovers the intended Persian text |
| Forms accept Persian / Arabic / Latin digits | Unify everything with `ToLatinDigits()` or `ToPersianDigits()` |
| Search must ignore `ي` vs `ی` | `SameAs` and `Fingerprint` canonicalize before compare |
| Logs must not leak card or mobile numbers | `SensitiveData.Mask` / `Redact` |
| Cheque or receipt needs amount in words | `NumberToWords` / `Cheque.ToWords` |
| Free text contains a mobile and an email | `ExtractValues()` finds and validates them |
| Import from Excel / OCR is messy | `NormalizeText()` cleans letters, digits and marks |

---

## Supported languages

| Code | Language | Script / digits |
|------|----------|-----------------|
| `fa` | Persian | Persian letters, digits ۰–۹ |
| `ar` | Arabic | Arabic letters, digits ٠–٩ |
| `en` | English | Latin letters, digits 0–9 |
| `sorani` / `ckb` | Kurdish Sorani | Arabic-based Kurdish script |
| `kurmanji` / `kmr` | Kurdish Kurmanji | Latin-based Kurdish |

You can register more languages at runtime with `LanguageRegistry` (see [Extensibility](#extensibility)).

---

## Quick examples

### Convert text and digits

```csharp
var mixed = "١٢٣ علي و ۱۲۳";

Text.ToPersian(mixed);   // ۱۲۳ علی و ۱۲۳
Text.ToArabic(mixed);    // Arabic script + Arabic-Indic digits
Text.ToEnglish(mixed);   // 123 ali u 123  (transliteration, not translation)

"۱۲۳۴۵".ToLatinDigits();    // 12345
"12345".ToPersianDigits();  // ۱۲۳۴۵
"12345".ToArabicDigits();   // ١٢٣٤٥
```

### Fix wrong keyboard

```csharp
"jsj".FixPersianKeyboard();   // تست
"sghl".FixPersianKeyboard();  // سلام
```

### Normalize dirty input

> **Important:** use `Text.Normalize` or `.NormalizeText()`.  
> Do **not** use BCL `string.Normalize()` — that only applies Unicode NFC/NFD and will not convert `ك` → `ک`.

```csharp
Text.Normalize("  كد ‌ ۱۲۳  ");  // کد 123
"علي".NormalizeText();           // علی
```

### Numbers ↔ words

```csharp
Text.NumberToWords(1250, "fa");        // یک هزار و دویست و پنجاه
Text.NumberToWords(1250, "en");        // one thousand two hundred fifty
Text.NumberToWords(1250, "sorani");    // یەک هەزار و دوو سەد و پەنجا
Text.NumberToWords(1250, "kurmanji");  // yek hezar û du sed û pêncî

Text.WordsToNumber("یک هزار و دویست", out var n);  // 1200
Text.TranslateNumberWords("یک هزار", "en");        // one thousand

Cheque.ToWords(12_000_000);  // دوازده میلیون تمام
```

### Iranian identifiers

```csharp
"09121111111".IsMobile();                    // true
IranianMobile.Format("09121111111");         // 0912-111-1111
IranianNationalCode.IsValid("0011111111");

var card = "6037991111111111";
BankCard.IsValid(card);                      // Luhn check
BankCard.DetectBank(card);                   // بانک ملی
```

### Extract from free text

```csharp
var text = "موبایل 09121111111 ایمیل a@b.com مبلغ ۲ میلیون تومان";

foreach (var hit in text.ExtractValues())
{
    Console.WriteLine($"{hit.Type}: {hit.Value} (valid={hit.IsValid})");
}
// IranianMobile, Email, Money, …
```

### Search-friendly comparison

```csharp
"علي".SameAs("علی");   // true  (ي vs ی)
"١٢٣".SameAs("123");   // true  (digit systems)
"علی".Fingerprint();   // stable key for indexes
```

### Mask sensitive data

```csharp
var line = "کارت 6037991111111111 موبایل 09121111111";

SensitiveData.Mask(line);
// کارت 6037-****-****-5670 موبایل 0912***1111

SensitiveData.Redact(line);
// کارت [REDACTED] موبایل [REDACTED]
```

### Custom patterns

```csharp
var pattern = PatternBuilder.Create("CustomerCode")
    .StartsWith("CUS-")
    .Digits(8)
    .Build();

pattern.IsMatch("CUS-12345678");  // true
pattern.IsMatch("CUS-12");        // false
```

### Dates and money

```csharp
"فردا".ToDate(DateTime.Today);

if ("۲.۵ میلیون تومان".TryMoney(out var money))
{
    // money.Amount = 2500000, money.Currency = Toman
}
```

### Kurdish

```csharp
"١٢٣ سڵاو".ConvertTo("sorani");
"سڵاو".ConvertTo("kurmanji");        

KurdishWords.ToSorani(42);
KurdishWords.ToKurmanji(42);
KurdishWords.TryParseSorani("یەک هەزار", out var n);
```

---

## Main APIs

### Text conversion & normalization

| API | Purpose |
|-----|---------|
| `Text.ToPersian` / `.ToPersian()` | Persian letters + Persian digits |
| `Text.ToArabic` / `.ToArabic()` | Arabic letters + Arabic-Indic digits |
| `Text.ToEnglish` / `.ToEnglish()` | Latin transliteration + Latin digits |
| `Text.ConvertTo("fa"\|"ar"\|…)` | Convert by language code |
| `Text.Normalize` / `.NormalizeText()` | Canonicalize letters, digits, invisible marks |
| `Text.Transliterate` / `.Transliterate()` | Script → Latin only |
| `.ToLatinDigits()` / `.ToPersianDigits()` / `.ToArabicDigits()` | Digit systems |
| `.FixPersianKeyboard()` / `Text.Keyboard(...)` | Wrong keyboard layout |

### Numbers & money

| API | Purpose |
|-----|---------|
| `Text.NumberToWords(n, "fa"\|"en"\|"sorani"\|"kurmanji")` | Number → words |
| `Text.WordsToNumber(...)` | Words → number |
| `Text.TranslateNumberWords(...)` | Words in language A → language B |
| `Cheque.ToWords` | Amount in words for cheques |
| `.TryMoney()` / `.TryNumber()` | Parse money and numbers from text |

### Iran & validation

| API | Purpose |
|-----|---------|
| `.IsMobile()` / `IranianMobile.*` | Iranian mobile |
| `IranianNationalCode.*` | National ID |
| `BankCard.*` / `.IsBankCard()` | Shetab card + Luhn + bank name |
| `Iban.*` | Iranian IBAN (Sheba) |
| `.IsEmail()` / `.IsUrl()` | Common formats |

### Extraction, search, security

| API | Purpose |
|-----|---------|
| `.ExtractValues()` / `TextExtractor.Extract` | Find values in free text |
| `.SameAs()` / `.Fingerprint()` | Semantic equality and search keys |
| `SensitiveData.Mask` / `Redact` | Hide sensitive spans |
| `PatternBuilder` | Build validation patterns without full regex |
| `SmartValue.Analyze` | Detect type + normalize + confidence |


---

## Optional packages

| Package | Role |
|---------|------|
| **Hs.LangSense** | Core library |

---

## Design notes

- **`ToEnglish` / `ConvertTo("kurmanji")` are transliteration**, not machine translation. For semantic translation use `Text.Translate` with a provider.
- **`NormalizeText` ≠ `string.Normalize`**. The BCL method does not apply Persian letter rules.
- Bank card `IsValid` means **checksum (Luhn) passed**, not that the card is active at the bank.
- Built-in bank BIN list includes major Iranian banks (e.g. Melli `603799`).

---

## Sample project

```bash
dotnet run --project examples/Hs.LangSense.Examples
```


---

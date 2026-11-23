# PayLense (EasyPay) Backend — Complete 32-Table Database Schema

نسخة الباك إند الكاملة لمشروع **PayLense (EasyPay)** المبنية بـ **ASP.NET Core (.NET 10) + Entity Framework Core + SQL Server** مع نظام الـ Authentication والـ **32 جدول** كاملين حسب ملف `EasyPay_Database_Schema.pdf`.

## الجداول المضافة والمفعلة في `AppDbContext` (32 جدول + جداول Identity)

1. **إدارة المستخدمين والأمان والجلسات والإعدادات:**
   - `AspNetUsers` (`ApplicationUser` — يشمل خصائص `Users`: `IsActive`, `IsDeleted`, `DeletedAt`, `DeletionReason`, `CreatedAt`, `UpdatedAt`)
   - `UserContactMethods`
   - `AuthenticationChallenges`
   - `ContactVerificationChallenges`
   - `LoginSessions`
   - `PhoneChangeRequests`
   - `EmailChangeRequests`
   - `AppSettings`
   - `AuditLogs`
   - بالإضافة لجداول الـ Auth الشغالة: `OtpCodes`, `RefreshTokens`, `TrustedDevices`

2. **الحسابات المالية والكروت البنكية وحدود الصرف:**
   - `FinancialAccounts`
   - `BankCards` (مربوط بـ `FinancialAccounts` مع تشفير `AES-GCM` + `CardFingerprint`, `CardToken`, `Currency`, `CardStatus`)
   - `SpendingLimits`

3. **التصنيفات والتحويلات والـ Ledger:**
   - `Categories`
   - `Transactions` (مع `RowVersion` و `IdempotencyKey` وكافة الـ `CHECK` Constraints)
   - `LedgerEntries` (`Debit` / `Credit`)
   - `TransactionStatusHistory`
   - `TransactionOtps`
   - `DelayedSends`

4. **طلبات الأموال والتحويلات الدورية التلقائية:**
   - `MoneyRequests`
   - `AutomatedSends`
   - `AutomatedSendRuns`

5. **تقسيم الفواتير والـ QR Bills:**
   - `Bills`
   - `BillParticipants` (Composite PK: `BillId` + `UserId`)
   - `BillSettlements`

6. **الإيصالات الذكية والتقارير:**
   - `Receipts` (مع `SuggestedCategoryId` و `AiConfidence`)
   - `ReportSnapshots`

7. **الإشعارات وتفضيلاتها:**
   - `Notifications`
   - `NotificationPreferences`

8. **الشات بوت والأسئلة الشائعة متعددة اللغات:**
   - `ChatSessions`
   - `ChatMessages`
   - `QA_Items`
   - `QA_Translations` (Composite PK: `QaId` + `Language`)

## خطوات تشغيل وتحديث قاعدة البيانات (SQL Server)

من داخل مجلد الحل الرئيسي، شغّل الأمر التالي لتطبيق الـ Migration وإنشاء جميع الجداول على SQL Server:

```bash
dotnet ef database update --project src/PayLense.Infrastructure --startup-project src/PayLense.API
```

أو من داخل **Package Manager Console** في Visual Studio 2022 (مع اختيار `PayLense.Infrastructure` كـ Default Project):

```powershell
Update-Database
```

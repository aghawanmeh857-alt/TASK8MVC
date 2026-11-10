# TASK8MVC

مشروع تاسك 8 باستخدام ASP.NET Core MVC وIdentity وبنفس أسلوب المرجع: Controllers وDTOs وViewBag وUserManager وRoleManager والتعامل المباشر مع ApplicationDbContext.

افتح `TASK8MVC.slnx` في Visual Studio ثم شغّل المشروع. في وضع Development تُطبَّق الـMigrations الموجودة تلقائيًا على قاعدة LocalDB المحددة في appsettings.json.

لإعداد أول SuperAdmin، افتح PowerShell داخل مجلد الحل وشغّل:

```powershell
powershell -ExecutionPolicy Bypass -File .\Setup-SuperAdmin.ps1
```

أدخل بريد الحساب وكلمة مرور تحقق شروط Identity. تُستخدم البيانات في جلسة التشغيل فقط، ولا تُحفظ كلمة المرور في ملفات المشروع. إذا كان هناك SuperAdmin بالفعل، لا ينشئ الإعداد مديرًا إضافيًا. بعد الإعداد يمكن تشغيل المشروع من Visual Studio كالمعتاد.

كل تسجيل عادي يحصل على دور User تلقائيًا. يستطيع SuperAdmin تغيير الأدوار من Manage Users. Admin وSuperAdmin يديران المنتجات ويراجعان الطلبات، والمستخدم يرى طلباته فقط.

يمكن إنشاء طلب بعدة منتجات، وتعديل الطلب أو حذفه أثناء Pending. تُحسب الأسعار على الخادم، وتحفظ أسعار الأصناف وقت الطلب. يوافق المسؤول على الطلب أو يرفضه من Details. لا يُحذف منتج مستخدم في طلب حفاظًا على تفاصيل الطلب. المخزون يُستخدم للتحقق من كمية كل صنف؛ لا توجد حركة خصم مخزون لأن التاسك لم يطلبها.

لم تتم إضافة جداول User أو Role جديدة. الاسم الكامل محفوظ كـClaim في جداول Identity الموجودة.

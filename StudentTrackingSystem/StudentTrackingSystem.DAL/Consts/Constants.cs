namespace StudentTrackingSystem.DAL.Consts
{
    // تحتوي هذه الفئة على ثوابت تُستخدم لتحديد أدوار المستخدمين في النظام.
    public static class Constants
    {
        public static string AdminRoleId { get; set; } = Guid.Parse("7f573ac6-5745-45a4-8cdb-680493538339").ToString("D");
        public static string TeatcherRoleId { get; set; } = Guid.Parse("2296757b-7b46-424a-affc-0d631a5c08b5").ToString("D");
        public static string StudentRoleId { get; set; } = Guid.Parse("9f3d6f4c-e2a4-48b9-b57f-3f1872e2b1ac").ToString("D");
    }
}

namespace Mashawer.Data.Helpers
{
    public static class EmailBodyBuilder
    {
        public static string GenerateEmailBody(string template, Dictionary<string, string> templateModel)
        {
            // لو الملف داخل Templates في الـ AGECS_Licensing.Data
            var templateFileName = $"{template}.html";
            var templatePath = Path.Combine(AppContext.BaseDirectory, "Templates", templateFileName);

            if (!File.Exists(templatePath))
                throw new FileNotFoundException($"Template not found at path: {templatePath}");

            var templateContent = File.ReadAllText(templatePath);

            foreach (var item in templateModel)
                templateContent = templateContent.Replace(item.Key, item.Value);

            return templateContent;
        }
    }


}

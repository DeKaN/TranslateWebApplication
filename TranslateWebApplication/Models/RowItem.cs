namespace TranslateWebApplication.Models
{
    using System.ComponentModel.DataAnnotations;

    public class RowItem
    {
        [DataType("bool")]
        [Display(Name = "Отредактирован")]
        public bool IsChanged { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Код")]
        public string Id { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Шаблон сообщения")]
        public string TemplateText { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Текущий перевод")]
        public string Translate { get; set; }
        [DataType(DataType.Text)]
        [Display(Name = "Новый перевод")]
        public string NewTranslate { get; set; }
        public string TemplateLang { get; set; }
    }
}

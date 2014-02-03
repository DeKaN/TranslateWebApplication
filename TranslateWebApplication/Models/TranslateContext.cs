namespace TranslateWebApplication.Models
{
    using System.Collections.Generic;
    using System.Linq;

    public class TranslateContext : List<RowItem>
    {
        public TranslateContext()
        {
        }

        public TranslateContext(IEnumerable<RowItem> items)
            : base(items)
        {
        }

        public string Value { get; set; }
        public bool IsReadOnly { get; set; }

        public RowItem this[string id]
        {
            get
            {
                return this.Single(item => item.Id == id);
            }
        }
    }
}
namespace TranslateWebApplication.Models
{
    using System.Collections.Generic;
    public class ViewTableData : List<TranslateContext>
    {
        public bool IsReadOnly { get; private set; }

        public ViewTableData(bool isReadOnly = false)
        {
            IsReadOnly = isReadOnly;
        }

        public ViewTableData(IEnumerable<TranslateContext> data, bool isReadOnly = false)
            : base(data)
        {
            IsReadOnly = isReadOnly;
        }
    }
}
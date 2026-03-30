namespace Diet_calulator.Models
{
    public class SavedCalculation
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Type { get; set; } // "Metabolism" or "Macro"
        public DateTime SavedDate { get; set; }
        public Dictionary<string, string> Data { get; set; } = new();

        public SavedCalculation() { }

        public SavedCalculation(string name, string type)
        {
            Name = name;
            Type = type;
            SavedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"{Name} - {SavedDate:yyyy-MM-dd HH:mm}";
        }
    }
}

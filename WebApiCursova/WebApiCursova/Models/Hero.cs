namespace WebApiCursova.Models
{
        public class Hero
        {
            public List<Heroes> Heroes { get; set; }
            public Hero()
            {
                Heroes = new List<Heroes>();
            }
        }

        public class Heroes
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Localized_name { get; set; }
            public string Primary_attr { get; set; }
            public string Attack_type { get; set; }
            public List<string> Roles { get; set; }  
        }
}

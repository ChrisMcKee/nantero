namespace Nantero
{
    public class NanteroConfig
    {
        private NanteroConfig() { }

        public static NanteroConfig Instance = new NanteroConfig
        {
            IteroBaseUrl = "http://itero.demo.pactas.com/api/v1/",
            NanteroBaseUrl = "http://nantero.apphb.com/",
            EntityId = "535f96de51f459a4ec5fffe7",
            PrivateAppId = "52a9fe4151f4595d4c69149a",
            PrivateAppSecret = "2b5b2d1b53d903d5a54fee92ed1072ed",
            PlanVariantId = "529f20ed51f4591c2000e946"
        };

        //public static NanteroConfig Instance = new NanteroConfig
        //{
        //    IteroBaseUrl = "http://localhost/api/v1/",
        //    NanteroBaseUrl = "http://localhost:11000/",
        //    EntityId = "5277bd65eb596a0ee8bd7832",
        //    PrivateAppId = "52a740c8eb596a0270233bc4",
        //    PrivateAppSecret = "19d9a6cc04855da27a7981ce9af32e20",
        //    PlanVariantId = "527caacdeb596a247c6e0500"
        //};
        
        public string NanteroBaseUrl { get; private set; }
        public string IteroBaseUrl { get; private set; }
        public string PrivateAppId { get; private set; }
        public string PrivateAppSecret { get; private set; }
        public string EntityId { get; private set; }
        public string PlanVariantId { get; set; }
    }
}

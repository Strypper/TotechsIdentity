namespace Entities
{
    public class Country : BaseEntity
    {
        public string CountryName    { get; set; }
        public string FlagUrl        { get; set; }
        public string BackgroundUrl  { get; set; }
        public string DevStory       { get; set; }
        public float  DevPercentages { get; set; }
    }
}

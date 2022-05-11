namespace TotechsIdentity.DataObjects
{
    public class CountryDTO : BaseDTO
    {
        public string CountryName    { get; set; }
        public string FlagUrl        { get; set; }
        public string BackgroundUrl  { get; set; }
        public string DevStory       { get; set; }
        public float  DevPercentages { get; set; }
    }
}

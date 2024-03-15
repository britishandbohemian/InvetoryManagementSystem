namespace InventoryAPI.Model.CustomizationModel
{
    // This class represents system customization data.

    public class Customization
    {
        // The unique identifier for system customization data.
        // Checked and confirmed as correct.
        public int Id { get; set; }

        // The name of the selected theme for the system's visual appearance.
        public string ThemeName { get; set; }

        // The location or path to an image used in the system's customization.
        public string ImageLocation { get; set; }

        // The percentage by which the product's cost price is marked up for selling.
        public int ProductMarkupPercentage { get; set; }

        // The currency used within the system for pricing and monetary values.
        public string Currency { get; set; }
    }
}

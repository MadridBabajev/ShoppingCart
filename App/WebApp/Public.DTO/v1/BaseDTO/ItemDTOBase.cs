namespace Public.DTO.v1.BaseDTO;

public class ItemDtoBase
{
    /// <summary>
    /// Unique identifier of the item.
    /// </summary>
    public Guid Id { get; set; }
    /// <summary>
    /// Name of the item.
    /// </summary>
    public string Name { get; set; } = default!;
    /// <summary>
    /// Price of the item.
    /// </summary>
    public int Price { get; set; } = default!;
    /// <summary>
    /// Rating of the item.
    /// </summary>
    public double Rating { get; set; } = default!;
    /// <summary>
    /// Picture of the item (if present).
    /// </summary>
    public byte[]? SubjectPicture { get; set; }
}
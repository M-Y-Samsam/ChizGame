namespace api.Helpers;

public class PaginationParams
{
    private const int MaxPageSize = 50; // field/variable
    private const int MinPageSize = 5;

    public int PageNumber { get; init; } = 1; // Short property

    private int _pageSize = 5; // Full property
    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            if (value > MaxPageSize)
            {
                _pageSize = MaxPageSize;
            }
             
            if (value < MinPageSize)
            {
                _pageSize = MinPageSize;
            }

            else
            {
                _pageSize = value ;
            }
        }
    }
}
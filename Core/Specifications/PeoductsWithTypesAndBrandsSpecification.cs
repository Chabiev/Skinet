using Core.Entities;

namespace Core.Specifications;

public class PeoductsWithTypesAndBrandsSpecification : BaseSpecification<Product>
{
    public PeoductsWithTypesAndBrandsSpecification()
    {
        AddInclude(x => x.ProductType);
        AddInclude(x => x.ProductBrand);
    }

    public PeoductsWithTypesAndBrandsSpecification(int id) 
        : base(x => x.Id == id)
    {
        AddInclude(x => x.ProductType);
        AddInclude(x => x.ProductBrand);
    }
}
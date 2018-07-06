
namespace VPS.Common.Specifications
{
    public interface ISpecification<in TCandidate>
    {
        bool IsSatisfiedBy(TCandidate candidate);
	}
}

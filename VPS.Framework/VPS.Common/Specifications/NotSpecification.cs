namespace VPS.Common.Specifications
{
    public class NotSpecification<TCandidate> : ISpecification<TCandidate>
    {
        private readonly ISpecification<TCandidate> specification;

        public NotSpecification(ISpecification<TCandidate> specification)
        {
            this.specification = specification;
        }
        public  bool IsSatisfiedBy(TCandidate obj)
        {
            return !specification.IsSatisfiedBy(obj);
        }
    }
}
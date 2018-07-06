namespace VPS.Common.Specifications
{
    public class AndSpecification<TCandidate> : ISpecification<TCandidate>
    {
        private readonly ISpecification<TCandidate> leftSide;
        private readonly ISpecification<TCandidate> rightSide;

        public AndSpecification(ISpecification<TCandidate> leftSide, ISpecification<TCandidate> rightSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
        }

        public bool IsSatisfiedBy(TCandidate candidate)
        {
            return leftSide.IsSatisfiedBy(candidate) && rightSide.IsSatisfiedBy(candidate);
        }
    }
}
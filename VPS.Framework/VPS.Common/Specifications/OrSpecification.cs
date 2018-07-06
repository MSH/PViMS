namespace VPS.Common.Specifications
{
    public class OrSpecification<TCandidate> : ISpecification<TCandidate>
    {
        private readonly ISpecification<TCandidate> leftSide;
        private readonly ISpecification<TCandidate> rightSide;

        public OrSpecification(ISpecification<TCandidate> leftSide, ISpecification<TCandidate> rightSide)
        {
            this.leftSide = leftSide;
            this.rightSide = rightSide;
        }

        public  bool IsSatisfiedBy(TCandidate obj)
        {
            return leftSide.IsSatisfiedBy(obj) || rightSide.IsSatisfiedBy(obj);
        }
    }
}
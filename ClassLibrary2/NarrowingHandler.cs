namespace ClassLibrary2
{
    public class NarrowingHandler<TIn, TOut> :IHandler<TIn> 
        where TOut : class, TIn
    {
        public IHandler<TOut> Handler { get; private set; }

        public NarrowingHandler(IHandler<TOut> handler)
        {
            Handler = handler;
        }

        public bool Handle(TIn message)
        {
            var casted = message as TOut;
            if (casted != null)
            {
                Handler.Handle(casted);
            }
            return true;
        }
    }
}
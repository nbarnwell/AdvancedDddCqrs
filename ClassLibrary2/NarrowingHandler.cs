namespace ClassLibrary2
{
    public class NarrowingHandler<TIn, TOut> :IHandler<TIn> 
        where TOut : class, TIn
    {
        private readonly IHandler<TOut> _handler;

        public NarrowingHandler(IHandler<TOut> handler)
        {
            _handler = handler;
        }

        public bool Handle(TIn message)
        {
            var casted = message as TOut;
            if (casted != null)
                _handler.Handle(casted);

            return true;
        }
    }
}
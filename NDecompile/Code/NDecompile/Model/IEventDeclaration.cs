
namespace LittleNet.NDecompile.Model
{
	public interface IEventDeclaration : IEventReference, IMemberDeclaration
	{
        /// <summary>
        /// Gets the reference to the method that adds delegates to the event
        /// </summary>
		IMethodReference AddMethod
		{
			get;
		}

        /// <summary>
        /// Gets the reference to the method that removes delegates from the event.
        /// </summary>
		IMethodReference RemoveMethod
		{
			get;
		}

        /// <summary>
        /// Gets the type of the handler
        /// </summary>
        ITypeReference EventType
        {
            get;
        }
	}
}

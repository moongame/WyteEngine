
namespace WyteEngine.Entities.AI
{
	public abstract class AINodeBase
	{
		protected Entity context;
		public abstract bool Run(Entity context);
	}
}

using System.Threading.Tasks;

namespace TalentFlow.Application.Common.Interfaces
{
    public interface IMessageBus
    {
        Task PublishAsync<T>(T message) where T : class;
    }
}

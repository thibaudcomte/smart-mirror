using System.Threading.Tasks;

namespace SmartMirror.App.Models
{
    public interface IDataProvider
    {
        Task<bool> OpenAsync();

        void Refresh();
    }
}

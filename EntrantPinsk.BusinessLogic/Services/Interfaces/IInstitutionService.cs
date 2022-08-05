using EntrantPinsk.Common.DTOs;

namespace EntrantPinsk.BusinessLogic.Services.Interfaces
{
    public interface IInstitutionService
    {
        IEnumerable<InstitutionDto> Get();
        InstitutionDto Get(int id);
    }
}

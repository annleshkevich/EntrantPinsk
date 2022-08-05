using EntrantPinsk.Common.DTOs;

namespace EntrantPinsk.BusinessLogic.Services.Interfaces
{
    public interface IEducationService
    {
        IEnumerable<EducationDto> Get();
        EducationDto Get(int id);
    }
}

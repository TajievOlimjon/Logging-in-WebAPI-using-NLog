namespace WebApi
{
    public interface IStudentService
    {
        Task<List<GetStudentDto>> GetAllStudentsAsync(StudentFilter filter, CancellationToken cancellationToken = default);
        Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken = default);
        Task<Response<string>> DeleteStudentAsync(int studentId, CancellationToken cancellationToken = default);
        Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model, CancellationToken cancellationToken = default);
        Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model, CancellationToken cancellationToken = default);
    }
}


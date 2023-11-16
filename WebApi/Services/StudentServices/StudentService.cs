using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Net;

namespace WebApi
{
    public class StudentService : IStudentService
    {
        private readonly ApplicationDbContext _dbContext;
        public StudentService(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<Response<AddStudentDto>> AddStudentAsync(AddStudentDto model, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s=>s.PhoneNumber==model.PhoneNumber,cancellationToken);
            if (student != null) return new Response<AddStudentDto>(HttpStatusCode.BadRequest, "A student with this number already exists");

            var newStudent = new Student
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Age = model.Age,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber
            };

            await _dbContext.Students.AddAsync(newStudent,cancellationToken);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0 
                ? new Response<AddStudentDto>(HttpStatusCode.OK, "Student data successfully added !")
                : new Response<AddStudentDto>(HttpStatusCode.OK, "Student data not added !");
        }

        public async Task<Response<string>> DeleteStudentAsync(int studentId, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken);
            if (student == null) return new Response<string>(HttpStatusCode.NotFound, "Student not found !");

            _dbContext.Students.Remove(student);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0
                ? new Response<string>(HttpStatusCode.OK, "Student data successfully deleted !")
                : new Response<string>(HttpStatusCode.OK, "Student data not deleted !");
        }

        public async Task<List<GetStudentDto>> GetAllStudentsAsync(StudentFilter filter, CancellationToken cancellationToken)
        {
            var query = _dbContext.Students.OrderBy(x=>x.Id).AsQueryable();

            if (filter.Query != null)
            {
                query = query.Where(x=>x.FirstName.ToLower().Contains(filter.Query.ToLower()) ||
                                       x.LastName.ToLower().Contains(filter.Query.ToLower())||
                                       x.PhoneNumber == filter.Query);
            }

            var students = await query.Select(student => new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            }).ToListAsync(cancellationToken);

            return students;
        }

        public async Task<Response<GetStudentDto>> GetStudentByIdAsync(int studentId, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.FirstOrDefaultAsync(s => s.Id == studentId,cancellationToken);
            if (student == null) return new Response<GetStudentDto>(HttpStatusCode.NotFound, "Student not found !");
                
            var model = new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Age = student.Age,
                Email = student.Email,
                PhoneNumber = student.PhoneNumber
            };

            return new Response<GetStudentDto>(HttpStatusCode.OK, "Student data", model);
        }

        public async Task<Response<UpdateStudentDto>> UpdateStudentAsync(UpdateStudentDto model, CancellationToken cancellationToken)
        {
            var student = await _dbContext.Students.AsNoTracking().FirstOrDefaultAsync(s => s.Id == model.Id,cancellationToken);
            if (student == null) return new Response<UpdateStudentDto>(HttpStatusCode.NotFound, "Student not found !");

            student.FirstName = model.FirstName;
            student.LastName = model.LastName;
            student.Age = model.Age;
            student.Email = model.Email;
            student.PhoneNumber = model.PhoneNumber;

            _dbContext.Students.Update(student);
            var result = await _dbContext.SaveChangesAsync(cancellationToken);

            return result > 0
                ? new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data successfully updated !")
                : new Response<UpdateStudentDto>(HttpStatusCode.OK, "Student data not updated !");
        }
    }
}

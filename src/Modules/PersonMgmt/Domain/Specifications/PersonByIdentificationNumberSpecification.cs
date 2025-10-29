using Core.Domain.Specifications;
using PersonMgmt.Domain.Aggregates;

namespace PersonMgmt.Domain.Specifications;

public class PersonByIdentificationNumberSpecification : Specification<Person>
{
    public PersonByIdentificationNumberSpecification(string identificationNumber)
    {
        Criteria = p => p.IsDeleted != true && p.IdentificationNumber == identificationNumber;
    }

    public PersonByIdentificationNumberSpecification(Guid excludeId, string identificationNumber)
    {
        Criteria = p => p.Id != excludeId && p.IsDeleted != true && p.IdentificationNumber == identificationNumber;
    }
}

public class PersonByEmailSpecification : Specification<Person>
{
    public PersonByEmailSpecification(string email)
    {
        Criteria = p => p.IsDeleted != true && p.Email == email;
    }

    public PersonByEmailSpecification(Guid excludeId, string email)
    {
        Criteria = p => p.Id != excludeId && p.IsDeleted != true && p.Email == email;
    }
}

public class PersonByStudentNumberSpecification : Specification<Person>
{
    public PersonByStudentNumberSpecification(string studentNumber)
    {
        AddInclude(p => p.Student);
        Criteria = p => p.IsDeleted != true && p.Student != null && p.Student.StudentNumber == studentNumber;
    }

    public PersonByStudentNumberSpecification(Guid excludeId, string studentNumber)
    {
        AddInclude(p => p.Student);
        Criteria = p =>
            p.Id != excludeId && p.IsDeleted != true && p.Student != null && p.Student.StudentNumber == studentNumber;
    }
}

public class PersonByEmployeeNumberSpecification : Specification<Person>
{
    public PersonByEmployeeNumberSpecification(string employeeNumber)
    {
        AddInclude(p => p.Staff);
        Criteria = p => p.IsDeleted != true && p.Staff != null && p.Staff.EmployeeNumber == employeeNumber;
    }

    public PersonByEmployeeNumberSpecification(Guid excludeId, string employeeNumber)
    {
        AddInclude(p => p.Staff);
        Criteria = p =>
            p.Id != excludeId && p.IsDeleted != true && p.Staff != null && p.Staff.EmployeeNumber == employeeNumber;
    }
}
using FluentValidation;
using Academic.Application.DTOs;

namespace Academic.Application.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Kurs kodu bo� olamaz")
            .Length(4, 20).WithMessage("Kurs kodu 4-20 karakter aras�nda olmal�d�r")
            .Matches(@"^[A-Z]{2,4}\d{2,4}$").WithMessage("Kurs kodu format hatas� (�rn: CS101)");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kurs ad� bo� olamaz")
            .Length(3, 200).WithMessage("Kurs ad� 3-200 karakter aras�nda olmal�d�r");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("A��klama maksimum 1000 karakter olabilir");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(20).WithMessage("ECTS maksimum 20 olabilir");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Kredi 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(10).WithMessage("Kredi maksimum 10 olabilir");

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 4).WithMessage("Seviye 1-4 aras�nda olmal�d�r");

        RuleFor(x => x.Type)
            .InclusiveBetween(1, 5).WithMessage("T�r 1-5 aras�nda olmal�d�r");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 3).WithMessage("Semester 1-3 aras�nda olmal�d�r");

        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Y�l 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(4).WithMessage("Y�l maksimum 4 olabilir");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("B�l�m ID'si bo� olamaz");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kurs ad� bo� olamaz")
            .Length(3, 200).WithMessage("Kurs ad� 3-200 karakter aras�nda olmal�d�r");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("A��klama maksimum 1000 karakter olabilir");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(20).WithMessage("ECTS maksimum 20 olabilir");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Kredi 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(10).WithMessage("Kredi maksimum 10 olabilir");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}

public class CancelCourseRequestValidator : AbstractValidator<CancelCourseRequest>
{
    public CancelCourseRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("�ptal nedeni bo� olamaz")
            .MaximumLength(500).WithMessage("�ptal nedeni maksimum 500 karakter olabilir");
    }
}

public class AddInstructorRequestValidator : AbstractValidator<AddInstructorRequest>
{
    public AddInstructorRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("��retmen ID'si bo� olamaz");
    }
}

public class AddPrerequisiteRequestValidator : AbstractValidator<AddPrerequisiteRequest>
{
    public AddPrerequisiteRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("�n ko�ul kurs ID'si bo� olamaz");

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (request.CourseId == request.PrerequisiteCourseId)
                    context.AddFailure("Bir kurs kendisinin �n ko�ulu olamaz");
            });
    }
}

public class RecordGradeRequestValidator : AbstractValidator<RecordGradeRequest>
{
    public RecordGradeRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Kay�t ID'si bo� olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester bo� olamaz");

        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara s�nav puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara s�nav puan� 100'den b�y�k olamaz");

        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puan� 100'den b�y�k olamaz");

        RuleFor(x => x.ECTS)
            .GreaterThanOrEqualTo(0).WithMessage("ECTS 0'dan k���k olamaz")
            .LessThanOrEqualTo(20).WithMessage("ECTS 20'den b�y�k olamaz");
    }
}

public class UpdateGradeRequestValidator : AbstractValidator<UpdateGradeRequest>
{
    public UpdateGradeRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si bo� olamaz");

        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara s�nav puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara s�nav puan� 100'den b�y�k olamaz");

        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puan� 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puan� 100'den b�y�k olamaz");
    }
}

public class SubmitGradeObjectionRequestValidator : AbstractValidator<SubmitGradeObjectionRequest>
{
    public SubmitGradeObjectionRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si bo� olamaz");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("�tirazi neden bo� olamaz")
            .MaximumLength(500).WithMessage("�tirazi neden maksimum 500 karakter olabilir");
    }
}

public class ApproveGradeObjectionRequestValidator : AbstractValidator<ApproveGradeObjectionRequest>
{
    public ApproveGradeObjectionRequestValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("�tiraz ID'si bo� olamaz");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty().WithMessage("�nceleyici ID'si bo� olamaz");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("�nceleme notlar� maksimum 1000 karakter olabilir");

        RuleFor(x => x.NewScore)
            .GreaterThanOrEqualTo(0).WithMessage("Yeni puan 0'dan k���k olamaz")
            .LessThanOrEqualTo(100).WithMessage("Yeni puan 100'den b�y�k olamaz")
            .When(x => x.NewScore.HasValue);
    }
}

public class EnrollStudentRequestValidator : AbstractValidator<EnrollStudentRequest>
{
    public EnrollStudentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester bo� olamaz");
    }
}

public class DropCourseRequestValidator : AbstractValidator<DropCourseRequest>
{
    public DropCourseRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ders b�rakma nedeni bo� olamaz")
            .MaximumLength(500).WithMessage("Ders b�rakma nedeni maksimum 500 karakter olabilir");
    }
}

public class JoinWaitingListRequestValidator : AbstractValidator<JoinWaitingListRequest>
{
    public JoinWaitingListRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("��renci ID'si bo� olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester bo� olamaz");
    }
}


public class ScheduleExamRequestValidator : AbstractValidator<ScheduleExamRequest>
{
    public ScheduleExamRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si bo� olamaz");

        RuleFor(x => x.ExamType)
            .InclusiveBetween(1, 5).WithMessage("S�nav t�r� 1-5 aras�nda olmal�d�r");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("S�nav tarihi bo� olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("S�nav tarihi format hatas� (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("S�nav tarihi ge�mi� tarih olamaz");
                }
                else
                    context.AddFailure("S�nav tarihi ge�ersiz");
            });

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Ba�lang�� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Ba�lang�� saati format hatas� (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Ba�lang�� saati ge�ersiz");
            });

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Biti� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Biti� saati format hatas� (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Biti� saati ge�ersiz");
            });

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (TimeOnly.TryParse(request.StartTime, out var start) &&
                    TimeOnly.TryParse(request.EndTime, out var end))
                {
                    if (end <= start)
                        context.AddFailure("Biti� saati ba�lang�� saatinden sonra olmal�d�r");
                }
            });

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan b�y�k olmal�d�r")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");

        RuleFor(x => x.IsOnline)
            .Custom((isOnline, context) =>
            {
                var request = context.InstanceToValidate;
                if (isOnline && string.IsNullOrWhiteSpace(request.OnlineLink))
                    context.AddFailure("Online s�nav i�in link sa�lanmal�d�r");
            });

        RuleFor(x => x.OnlineLink)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Online link ge�erli bir URL olmal�d�r")
            .When(x => !string.IsNullOrWhiteSpace(x.OnlineLink));
    }
}

public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequest>
{
    public UpdateExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("S�nav ID'si bo� olamaz");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("S�nav tarihi bo� olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("S�nav tarihi format hatas� (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("S�nav tarihi ge�mi� tarih olamaz");
                }
                else
                    context.AddFailure("S�nav tarihi ge�ersiz");
            });

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Ba�lang�� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Ba�lang�� saati format hatas� (HH:mm)");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Biti� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Biti� saati format hatas� (HH:mm)");
    }
}

public class PostponeExamRequestValidator : AbstractValidator<PostponeExamRequest>
{
    public PostponeExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("S�nav ID'si bo� olamaz");

        RuleFor(x => x.NewExamDate)
            .NotEmpty().WithMessage("Yeni s�nav tarihi bo� olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Yeni s�nav tarihi format hatas� (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Yeni s�nav tarihi ge�mi� tarih olamaz");
                }
                else
                    context.AddFailure("Yeni s�nav tarihi ge�ersiz");
            });

        RuleFor(x => x.NewStartTime)
            .NotEmpty().WithMessage("Yeni ba�lang�� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni ba�lang�� saati format hatas� (HH:mm)");

        RuleFor(x => x.NewEndTime)
            .NotEmpty().WithMessage("Yeni biti� saati bo� olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni biti� saati format hatas� (HH:mm)");
    }
}

public class CancelExamRequestValidator : AbstractValidator<CancelExamRequest>
{
    public CancelExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("S�nav ID'si bo� olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("�ptal nedeni bo� olamaz")
            .MaximumLength(500).WithMessage("�ptal nedeni maksimum 500 karakter olabilir");
    }
}

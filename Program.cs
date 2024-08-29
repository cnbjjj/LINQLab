using System.Buffers.Text;
using System.Runtime.Intrinsics.Arm;

namespace Lab2
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Employee> employees = new List<Employee>
            {
                new Employee { EmployeeID = 1, FirstName = "Michael", LastName = "Johnson", Salary = 75000, DepartmentID = 1, ProjectID = 1},
                new Employee { EmployeeID = 2, FirstName = "Emily", LastName = "Clark", Salary = 82000, DepartmentID = 2, ProjectID = 2},
                new Employee { EmployeeID = 3, FirstName = "David", LastName = "Martinez", Salary = 91000, DepartmentID = 3, ProjectID = 3},
                new Employee { EmployeeID = 4, FirstName = "Olivia", LastName = "Garcia", Salary = 85000, DepartmentID = 1, ProjectID = 4},
                new Employee { EmployeeID = 5, FirstName = "Daniel", LastName = "Smith", Salary = 98000, DepartmentID = 2, ProjectID = 5},
                new Employee { EmployeeID = 6, FirstName = "Sophia", LastName = "Davis", Salary = 102000, DepartmentID = 3, ProjectID = 1},
                new Employee { EmployeeID = 7, FirstName = "James", LastName = "Rodriguez", Salary = 115000, DepartmentID = 1, ProjectID = 2},
                new Employee { EmployeeID = 8, FirstName = "Isabella", LastName = "Hernandez", Salary = 128000, DepartmentID = 2, ProjectID = 3},
                new Employee { EmployeeID = 9, FirstName = "William", LastName = "Lopez", Salary = 137000, DepartmentID = 3, ProjectID = 4},
                new Employee { EmployeeID = 10, FirstName = "Ava", LastName = "Gonzalez", Salary = 145000, DepartmentID = 1, ProjectID = 5}
            };
            List<Department> departments = new List<Department>
            {
                new Department { DepartmentID = 1, DepartmentName = "HR" },
                new Department { DepartmentID = 2, DepartmentName = "IT" },
                new Department { DepartmentID = 3, DepartmentName = "Finance" }
            };
            List<Project> projects = new List<Project>
            {
                new Project { ProjectID = 1, ProjectName = "Employee Engagement Initiative", DepartmentID = 1 },
                new Project { ProjectID = 2, ProjectName = "Cybersecurity Upgrade", DepartmentID = 2 },
                new Project { ProjectID = 3, ProjectName = "Financial Systems Overhaul", DepartmentID = 3 },
                new Project { ProjectID = 4, ProjectName = "Recruitment Drive", DepartmentID = 1 },
                new Project { ProjectID = 5, ProjectName = "Data Center Expansion", DepartmentID = 2 }
            };


            void GroupByAndAggregate()
            {
                Console.WriteLine("\n\n------------------------------------GroupByAndAggregate-------------------------------------------");
                var deptGroup = employees
                    .Join(
                        departments,
                        employees => employees.DepartmentID,
                        departments => departments.DepartmentID,
                        (employee, department) => new
                        {
                            employee,
                            department
                        }
                    )
                    .GroupBy(e => e.employee.DepartmentID);

                // Group employees by their departments.
                Console.WriteLine("\n\n- Group employees by their departments: ");
                var result = deptGroup.Select(group => new
                {
                    DepartmentId = group.Key,
                    DepartmentName = group.First().department.DepartmentName,
                    Empoloyees = group.Select(e => e.employee)
                });
                foreach (var item in result)
                {
                    Console.WriteLine($"\n * Department ID: {item.DepartmentId}, DepartmentName: {item.DepartmentName}");
                    foreach (var employee in item.Empoloyees)
                    {
                        Console.WriteLine($"   Employee ID: {employee.EmployeeID}, Name: {employee.FirstName} {employee.LastName}, Salary: {employee.Salary}");
                    }
                }

                // Calculate the average salary for each department.
                Console.WriteLine("\n\n- Calculate the average salary for each department: \n");
                var resultDptAvg = deptGroup
                    .Select(
                        group => new
                        {
                            DepartmentId = group.Key,
                            DepartmentName = group.First().department.DepartmentName,
                            AverageSalary = group.Select(e => e.employee).Average(p => p.Salary)
                        }
                    );
                foreach (var item in resultDptAvg)
                {
                    Console.WriteLine($"   Department ID: {item.DepartmentId}, Department Name: {item.DepartmentName}, Average Salary: {item.AverageSalary}");
                }

                //Find the department with the highest total salary.
                Console.WriteLine("\n\n- Find the department with the highest total salary: \n");
                var resultHighestTotalDpt = deptGroup
                .OrderByDescending(group => group.Select(group => group.employee)
                .Sum(employee => employee.Salary))
                .FirstOrDefault();
                Console.WriteLine($"   Department ID: {resultHighestTotalDpt.Key}, Department Name: {resultHighestTotalDpt.Select(result => result.department).First().DepartmentName}, Total Salary: {resultHighestTotalDpt.Select(result => result.employee).Sum(employee => employee.Salary)}");

                //Group employees by the projects they are involved in.
                Console.WriteLine("\n\n- Group employees by the projects they are involved in: ");
                var projGroup = employees
                    .Join(
                        projects,
                        employee => employee.ProjectID,
                        project => project.ProjectID,
                        (employee, project) => new
                        {
                            employee,
                            project
                        }
                    )
                    .GroupBy(joinResult => joinResult.employee.ProjectID);

                var resultOfProjGroup = projGroup.Select(group => new
                {
                    ProjectId = group.Key,
                    ProjectName = group.First().project.ProjectName,
                    Employees = group.Select(group => group.employee)
                });

                foreach (var item in resultOfProjGroup)
                {
                    Console.WriteLine($"\n * Project ID: {item.ProjectId}, Project Name: {item.ProjectName}");
                    foreach (var employee in item.Employees)
                    {
                        Console.WriteLine($"   Employee ID: {employee.EmployeeID}, Name: {employee.FirstName} {employee.LastName}, Salary: {employee.Salary}");
                    }
                }

                //Calculate the total number of projects in each department.
                Console.WriteLine("\n\n- Calculate the total number of projects in each department: \n");
                var resultTotalProjInDpt = deptGroup
                    .Select(group => new
                    {
                        DepartmentId = group.Key,
                        DepartmentName = group.First().department.DepartmentName,
                        TotalPrjects = group.Select(e => e.employee.ProjectID).Distinct().Count()
                    });

                foreach (var item in resultTotalProjInDpt)
                {
                    Console.WriteLine($"   Department ID: {item.DepartmentId}, Department Name: {item.DepartmentName}, Total Projects: {item.TotalPrjects}");
                }

            }


            void PerformJoins()
            {
                //inner joins between the Employee, Department, and Project lists based on the relevant IDs
                //Output the result of the join operation, including information about employees, their departments, and projects
                Console.WriteLine("\n\n----------------------------------------PerformJoins-----------------------------------------------");
                Console.WriteLine("\n\n- Inner joins by the Employee: \n");
                var employeeResult = employees
                    .Join(
                    departments,
                    employee => employee.DepartmentID,
                    department => department.DepartmentID,
                    (employee, department) => new
                    {
                        employee,
                        department
                    })
                    .Join(
                    projects,
                    joinResult => joinResult.employee.ProjectID,
                    project => project.ProjectID,
                    (joinResult, project) => new
                    {
                        joinResult.employee,
                        joinResult.department,
                        project
                    });

                foreach (var item in employeeResult)
                {
                    Console.WriteLine($"   Employee ID: {item.employee.EmployeeID}, Name: {item.employee.FirstName} {item.employee.LastName}, Salary: {item.employee.Salary}, Department: {item.department.DepartmentName}, Project: {item.project.ProjectName}");
                }

                Console.WriteLine("\n\n- Inner joins by the Department: ");
                var departmentResult = departments
                    .Join(
                    employees,
                    department => department.DepartmentID,
                    employee => employee.DepartmentID,
                    (department, employee) => new
                    {
                        department,
                        employee
                    })
                    .Join(
                    projects,
                    joinResult => joinResult.employee.ProjectID,
                    project => project.ProjectID,
                    (joinResult, project) => new
                    {
                        joinResult.department,
                        joinResult.employee,
                        project
                    })
                    .GroupBy(joinResult => joinResult.department.DepartmentID);

                foreach (var result in departmentResult)
                {
                    Console.WriteLine($"\n * Department: {result.First().department.DepartmentName}");
                    foreach (var item in result)
                    {
                        Console.WriteLine($"   Employee ID: {item.employee.EmployeeID}, Name: {item.employee.FirstName} {item.employee.LastName}, Salary: {item.employee.Salary}, Project: {item.project.ProjectName}");
                    }
                }

                Console.WriteLine("\n\n- Inner joins by the Project: ");
                var projectResult = projects
                    .Join(
                    employees,
                    project => project.ProjectID,
                    employee => employee.ProjectID,
                    (project, employee) => new
                    {
                        project,
                        employee
                    })
                    .Join(
                    departments,
                    joinResult => joinResult.employee.DepartmentID,
                    department => department.DepartmentID,
                    (joinResult, department) => new
                    {
                        joinResult.project,
                        joinResult.employee,
                        department
                    })
                    .GroupBy(joinResult => joinResult.project.ProjectID);

                foreach (var result in projectResult)
                {
                    Console.WriteLine($"\n * Project: {result.First().project.ProjectName}");
                    foreach (var item in result)
                    {
                        Console.WriteLine($"   Employee ID: {item.employee.EmployeeID}, Name: {item.employee.FirstName} {item.employee.LastName}, Salary: {item.employee.Salary}, Department: {item.department.DepartmentName}");
                    }
                }
            }


            void FilterAndSelect()
            {
                //Filter employees based on specific conditions, such as employees with a salary above a certain threshold.
                //Select and display only the FirstName and LastName of employees and the ProjectName of the projects they are involved in.
                Console.WriteLine("\n\n---------------------------------------FilterAndSelect---------------------------------------------");
                var targetSalary = 100000;
                Console.WriteLine($"\n\n- Reuslt of employees with salary above {targetSalary}.\n");
                var employeeResult = employees
                    .Where(
                    employee => employee.Salary > targetSalary
                    )
                    .Join(
                    departments,
                    employee => employee.DepartmentID,
                    department => department.DepartmentID,
                    (employee, department) => new
                    {
                        employee,
                        department
                    })
                    .Join(
                    projects,
                    joinResult => joinResult.employee.ProjectID,
                    project => project.ProjectID,
                    (joinResult, project) => new
                    {
                        joinResult.employee,
                        joinResult.department,
                        project
                    });

                foreach (var item in employeeResult)
                {
                    Console.WriteLine($"   Employee ID: {item.employee.EmployeeID}, Name: {item.employee.FirstName} {item.employee.LastName}, Salary: {item.employee.Salary}, Department: {item.department.DepartmentName}, Project: {item.project.ProjectName}");
                }
            }



            //Run
            Console.Clear();
            GroupByAndAggregate();
            PerformJoins();
            FilterAndSelect();
        }
    }
}



using TodoLibrary.Models;

namespace TodoLibrary.DataAccess;

public class TodoData : ITodoData
{

    private readonly IsqlDataAccess _sql;
    public TodoData(IsqlDataAccess sql)
    {
        _sql = sql;
    }

    public Task<List<TodoModel>> GetAllAssigned(int assignedTo)
    {
        return _sql.LoadData<TodoModel, dynamic>(
              storedProcedure: "dbo.spTodos_getAllAssigned",
              new { AssignedTo = assignedTo },
              connectionStringName: "Default");
    }

    public async Task<TodoModel?> GetOneAssigned(int assignedTo, int todoId)
    {
        var results = await _sql.LoadData<TodoModel, dynamic>(
              storedProcedure: "dbo.spTodos_getOneAssigned",
              new { AssignedTo = assignedTo, TodoId = todoId },
              connectionStringName: "Default");

        return results.FirstOrDefault();
    }

    public async Task<TodoModel?> Create(int assignedTo, string task)
    {
        var results = await _sql.LoadData<TodoModel, dynamic>(
              storedProcedure: "dbo.spTodos_Create",
              new { AssignedTo = assignedTo, Task = task },
              connectionStringName: "Default");

        return results.FirstOrDefault();
    }

    public Task UpdateTask(int assignedTo, int todoId, string task)
    {
        return _sql.SaveData<dynamic>(
              storedProcedure: "dbo.spTodos_UpdateTask",
              new { AssignedTo = assignedTo, TodoId = todoId, Task = task },
              connectionStringName: "Default");
    }

    public Task CompleteTodo(int assignedTo, int todoId)
    {
        return _sql.SaveData<dynamic>(
              storedProcedure: "dbo.spTodos_CompleteTodo",
              new { AssignedTo = assignedTo, TodoId = todoId },
              connectionStringName: "Default");
    }

    public Task Delete(int assignedTo, int todoId)
    {
        return _sql.SaveData<dynamic>(
              storedProcedure: "dbo.spTodos_Delete",
              new { AssignedTo = assignedTo, TodoId = todoId },
              connectionStringName: "Default");
    }
}

namespace ToDoList.Models
{
    public class TasksToDo
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string PdfUrl { get; set; }
        public DateTime DeadLine { get; set; }

    }
}

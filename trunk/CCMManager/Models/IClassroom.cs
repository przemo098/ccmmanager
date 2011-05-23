namespace CCMManager.Models
{
    using Caliburn.Micro;

    public interface IClassroom
    {
        string Name { get; set; }
        BindableCollection<IComputer> Computers { get; set; }
    }
}

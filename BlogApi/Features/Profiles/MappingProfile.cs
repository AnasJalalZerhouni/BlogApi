namespace BlogApi.Features.Profiles
{
    public class MappingProfile : AutoMapper.Profile
    {
        public MappingProfile()
        {
            CreateMap<Domain.Person,Profile>(AutoMapper.MemberList.None);
        }
    }
}

using AutoMapper;
using BookStore.Application.DTOs;
using BookStore.Domain.Entities;

namespace BookStore.Application.MappingProfiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Book, BookDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category.Name))
            .ForMember(d => d.AuthorName, opt => opt.MapFrom(s => s.Author.Name))
            .ForMember(d => d.PublisherName, opt => opt.MapFrom(s => s.Publisher.Name));

        CreateMap<BookCreateDto, Book>();
        CreateMap<BookUpdateDto, Book>()
            .ForAllMembers(opt => opt.Condition((src, dest, val) => val != null));

        CreateMap<Order, OrderResponseDto>()
            .ForMember(d => d.Items, opt => opt.MapFrom(s => s.OrderItems))
            .ForMember(d => d.CustomerName, opt => opt.MapFrom(s => s.User != null ? s.User.FullName : ""));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(d => d.BookTitle, opt => opt.MapFrom(s => s.Book.Title));

        CreateMap<Category, CategoryDto>().ReverseMap();

        CreateMap<Review, ReviewDto>()
            .ForMember(d => d.BookTitle, opt => opt.MapFrom(s => s.Book.Title))
            .ForMember(d => d.UserName, opt => opt.MapFrom(s => s.User.FullName));
    }
}
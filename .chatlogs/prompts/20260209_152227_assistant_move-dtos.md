---
date: 2026-02-09T12:10:05Z
role: assistant
conversation_id: unknown
context: WroclawTheatreTickets repo
redacted: false
---

Split all DTO classes in [src/WroclawTheatreTickets.Application/Contracts/Dtos](src/WroclawTheatreTickets.Application/Contracts/Dtos) into individual files.

Removed combined files:
- ApiDtos.cs
- ReviewDto.cs (original with multiple classes)
- ShowDto.cs (original with multiple classes)
- UserDto.cs (original with multiple classes)

Created individual files:
- [RepertoireApiResponse.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/RepertoireApiResponse.cs)
- [RepertoireEventDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/RepertoireEventDto.cs)
- [RepertoireCategoryDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/RepertoireCategoryDto.cs)
- [AgeCategoryDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/AgeCategoryDto.cs)
- [ShowEventDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/ShowEventDto.cs)
- [StageDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/StageDto.cs)
- [BuildingDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/BuildingDto.cs)
- [ApiDtoMapper.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/ApiDtoMapper.cs)
- [ReviewDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/ReviewDto.cs)
- [CreateReviewRequest.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/CreateReviewRequest.cs)
- [NotificationDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/NotificationDto.cs)
- [ShowDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/ShowDto.cs)
- [ShowDetailDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/ShowDetailDto.cs)
- [UserDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/UserDto.cs)
- [UserProfileDto.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/UserProfileDto.cs)
- [UserRegistrationRequest.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/UserRegistrationRequest.cs)
- [UserLoginRequest.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/UserLoginRequest.cs)
- [OAuthRequest.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/OAuthRequest.cs)
- [AuthenticationResponse.cs](src/WroclawTheatreTickets.Application/Contracts/Dtos/AuthenticationResponse.cs)

TheatreDto.cs was already a single class file and left unchanged.

Build and test results:
- Build succeeded in 2.3s
- Tests: 66 passed, 0 failed

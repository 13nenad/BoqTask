#### My Approach

I completed the task using a .Net Core API in Visual Studio.

I understand that acceptance criteria should be fairly short and simple but this one is very vague and open to interpretation. Normally I'd go to a business analyst and clarify what is needed. The way I understood it is that this request should return session and topic information together. This in my opinion is unnecessary and the API is already broken up nicely into its resources and it would just be better to make 2 calls (a GetSession call and then a GetSessionTopics call). This combination would only be useful if this combination of calls is very frequent and you wanted to save some time by only sending one round trip instead of two. You're also sending less data over the wire this way, but not by much.

I decided to just go with the one generic model, where you can have items nested inside items. It can be more efficient (by having a shorter model and sending less data) if you had one model for the session and another for the topic but I thought I'd stick with the overall trend and try be generic.

I'm not sure if I was doing something inheritly wrong but I found that filtering didn't work on any of the provided API calls. So I performed all of the filtering myself after retrieving all of the sessions.

I added the SubscriptionKey in the AppSettings and retrieved it from there for both the controller and unit tests, to add some flexibility.

#### Assumptions
1. My main assumption was that a speaker can only present at any one given time, therefore if you filtered by speakername and dateTimeSlot then you should only ever get back one session.
2. I also assumed that these filtering options were mandatory and put them as url parameters, rather than optional query parameters.
3. I assumed that in the case of no session being found that a 404 Not Found should be returned.
4. For some reason not all sessions have a speaker in the data, for e.g. session/128. So I assumed that if this is the case these sessions will not be returned, in line with assumption 1.
5. I assumed that "all possible values" means absolutely everything, including links etc.

#### Unit Testing

I decided to break up the tasks of retrieving the target session and adding of the topics to that session, and made them static methods so I could easily unit test them.

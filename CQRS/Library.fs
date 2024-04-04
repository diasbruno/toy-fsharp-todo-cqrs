namespace CQRS

open System
open Database

type Handler<'a, 'd, 'r, 'e> = DataSource<'a, 'e> -> 'd -> Async<Result<'r, 'e>>

module Dispatch =
  type Reply<'a, 'b, 'e> =
    | Reply of 'a * AsyncReplyChannel<Result<'b, 'e>>
    | NoReply of 'a

  let sync (mail: MailboxProcessor<Reply<'a, 'b, 'e>>) a =
    mail.PostAndReply(fun reply -> Reply(a, reply))

  let noReply (mail: MailboxProcessor<Reply<'a, 'b, 'e>>) a =
    mail.Post(NoReply a)

type Command = CreateTodo of string

type Event = TodoCreated of Guid * string

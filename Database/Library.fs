namespace Database

open CQRS
open DomainError

type InMemory<'a>() =
  let mutable store : 'a list = []
  interface DataSource<'a, DomainError> with
    member this.All =
      async { return Result.Ok(store) }
    member this.Count =
      async { return Ok(store.Length) }
    member this.Store item =
      async {
       do store <- List.append [item] store
       return Ok(item)
     }

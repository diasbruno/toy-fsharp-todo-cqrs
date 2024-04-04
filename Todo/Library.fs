namespace Todo

open System

module Todo =
  type Id = Guid
  and t = { Id: Id; Title: string }

  let create (id: Id) (title: string) : t = { Id = id; Title = title }

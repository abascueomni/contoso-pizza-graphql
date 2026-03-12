import { gql } from "@apollo/client";

//Contains GraphQL queries for fetching pizza data
export const GET_PIZZAS = gql`
  query {
    pizzas {
      id
      name
      price
      isGlutenFree
      toppings {
        topping
      }
    }
  }
`;
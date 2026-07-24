import { ProductTypeEnum } from "../enums/product-type-enum"

export interface ProductType {
  id: string,
  name : string,
  description: string,
  price: number,
  pictureUrl: string,
  type: ProductTypeEnum,
  brand: string 
  quantitiyInStock: number
}

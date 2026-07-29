import { Routes } from '@angular/router';
import { Shop } from './features/shop/shop';
import { UpsertProduct } from './features/shop/upsert-product/upsert-product';
import { ProductDetails } from './features/shop/product-details/product-details';
import { Login } from './features/account/login/login';
import { Register } from './features/account/register/register';

export const routes: Routes = [
	{
		path: "",
		component: Shop
	},
	{
		path: "upsert",
		component: UpsertProduct
	},
	{
		path: "product/:id",
		component: ProductDetails
	},
	{
		path: "login",
		component: Login
	},
	{
		path: "register",
		component: Register
	}
];

import { Component, inject, OnInit, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Header } from "./shared/header/header";
import { ProductServices } from './services/ShopService';
import { Product } from './shared/models/product';
import { Shop } from "./features/shop/shop";

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, Header, Shop],
  templateUrl: './app.html',
  styleUrl: './app.css'
})

export class App {
  


}

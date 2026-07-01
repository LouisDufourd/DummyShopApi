
drop table if exists carts_products;
drop table if exists carts;
drop table if exists orders_products;
drop table if exists orders;
drop table if exists orders_status;
drop table if exists images;
drop table if exists products_categories;
drop table if exists categories;
drop table if exists products;
drop table if exists employees;
drop table if exists users;
drop table if exists addresses;
drop type if exists product_order_status;
drop type if exists roles;
drop extension if exists unaccent;

create extension unaccent;

create type product_order_status as enum ('none', 'picked', 'packed');
create type roles as enum ('inventory', 'packer', 'manager');

create table addresses(
	address_id			serial4 				not null	primary key,
	street_number		varchar(10) 			not null,
	street_name			varchar(200) 			not null,
	city				varchar(100) 			not null,
	postcode			varchar(20) 			not null,
	country				varchar(100) 			not null,
	created_at			timestamp 				not null	default now()
);

create table users(
	user_id						serial4 		not null	primary key,
	address_id_fk				int4			not null	references addresses(address_id)	unique,
	name						varchar(100),
	firstname					varchar(100),
	email						varchar(150) 	not null	unique,
	password					varchar(255)	not null,
	created_at					timestamp 		not null	default now(),
	admin						bool			not null,
	email_verification_token 	varchar(255),
	email_verified				bool			not null	default false
);

create table employees(
	user_id_fk					int4			not null	primary key	references users(user_id),
	role						roles			not null
);

create table products(
	product_id					serial4 		not null	primary key,
	name						varchar(255) 	not null,
	seller						varchar(100) 	not null,
	short_desc					varchar(250),
	description					text,
	discount					numeric(10,2),
	price						numeric(10,2) 	not null,
	sells_score					int4,
	quantity					int4,
	created_at					timestamp 		not null 	default now()
);

create table categories(
	category_id					serial4 		not null	primary key,
	name						varchar(100) 	not null,
	description					text 			not null
);

create table products_categories(
	product_id_fk				int4 			not null 	references products(product_id),
	category_id_fk				int4 			not null 	references categories(category_id)
);

create table images(
	image_id					serial4 		not null	primary key,
	product_id_fk				int4 			not null 	references products(product_id),
	url							VARCHAR(100) 	not null,
	alt							varchar(100),
	created_at					timestamp 		not null	default now()
);

create table orders_status(
	order_status_id		serial4  				not null	primary key,
	label				varchar(100) 			not null
);

create table orders(
	order_id			serial4					not null 	primary key,
	order_status_id_fk	int4					not null	references orders_status(order_status_id),
	user_id_fk			int4					not null	references users(user_id),
	created_at			timestamp				not null	default now()
);

create table orders_products(
	order_id_fk			int4	 				not null	references orders(order_id),
	product_id_fk		int4					not null	references products(product_id),
	quantity			int4					not null,
	status				product_order_status	not null	default 'none',
	constraint pk_orders_product primary key (order_id_fk, product_id_fk)
);

create table carts(
	cart_id 			serial4 				not null	primary key,
	user_id_fk			int4					not null 	references users(user_id)	unique,
	created_at			timestamp				not null	default now()
);

create table carts_products(
	product_id_fk		int4	 				not null 	references products(product_id),
	cart_id_fk			int4	 				not null 	references carts(cart_id),
	quantity			int4 					not null,
	constraint pk_cart_product primary key (product_id_fk, cart_id_fk)
);

-- ========
-- TRIGGERS
-- ========

create or replace function trg_carts_products_quantity_cleanup()
returns trigger as $$
begin
  if new.quantity <= 0 then
    delete from carts_products
    where product_id_fk = new.product_id_fk
      and cart_id_fk    = new.cart_id_fk;
  end if;

  return null;
end;
$$ language plpgsql;


drop trigger if exists on_carts_products_quantity_change ON carts_products;

create trigger on_carts_products_quantity_change
after update of quantity ON carts_products
for each row
execute function trg_carts_products_quantity_cleanup();

-- ============================================
-- SEED DATA (PostgreSQL) - Nouveau schéma
-- ============================================
BEGIN;

INSERT INTO orders_status(label) VALUES
('Panier'),
('Payée'),
('Expédiée'),
('Livrée'),
('Annulée');

-- =====================
-- ADDRESSES
-- =====================

INSERT INTO addresses(street_number, street_name, city, postcode, country)
VALUES
('12', 'Main Street', 'Paris', '75001', 'France'),
('25', 'Oak Avenue', 'Lyon', '69001', 'France'),
('8', 'Rue Victor Hugo', 'Marseille', '13001', 'France'),
('45', 'Boulevard Gambetta', 'Lille', '59000', 'France'),
('7', 'Rue Nationale', 'Toulouse', '31000', 'France'),
('18', 'High Street', 'London', 'SW1A', 'United Kingdom');

-- =====================
-- USERS
-- Password = "password"
-- BCrypt hash example
-- =====================

INSERT INTO users
(address_id_fk, name, firstname, email, password, admin, email_verified)
VALUES
(1,'Doe','John','john@example.com','$2a$11$7EqJtq98hPqEX7fNZaFWoOHiJ6jL2L6P7LQ9LqA5A6k0t4L4P4xAa',false,true),
(2,'Smith','Jane','jane@example.com','$2a$11$7EqJtq98hPqEX7fNZaFWoOHiJ6jL2L6P7LQ9LqA5A6k0t4L4P4xAa',true,true),
(3,'Martin','Paul','paul@example.com','$2a$11$7EqJtq98hPqEX7fNZaFWoOHiJ6jL2L6P7LQ9LqA5A6k0t4L4P4xAa',false,true),
(4,'Dupont','Emma','emma@example.com','$2a$11$7EqJtq98hPqEX7fNZaFWoOHiJ6jL2L6P7LQ9LqA5A6k0t4L4P4xAa',false,false),
(5,'Johnson','Mike','mike@example.com','$2a$11$7EqJtq98hPqEX7fNZaFWoOHiJ6jL2L6P7LQ9LqA5A6k0t4L4P4xAa',false,true),
(6,'Brown','Alice','alice@example.com','$2a$11$7EqJtq98hPqEX7fNZaFWoOHiJ6jL2L6P7LQ9LqA5A6k0t4L4P4xAa',false,true);

-- =====================
-- EMPLOYEES
-- =====================

INSERT INTO employees(user_id_fk, role)
VALUES
(2,'manager'),
(3,'inventory'),
(5,'packer');

-- =====================
-- CATEGORIES
-- =====================

INSERT INTO categories(name, description)
VALUES
('Electronics','Electronic devices'),
('Computers','Desktop and laptop computers'),
('Gaming','Gaming accessories'),
('Books','Books and novels'),
('Home','Home equipment'),
('Office','Office accessories');

-- =====================
-- PRODUCTS
-- =====================

INSERT INTO products
(name,seller,short_desc,description,discount,price,sells_score,quantity)
VALUES
('Gaming Mouse','Logitech','Wireless gaming mouse','High precision wireless mouse',10.00,79.99,500,40),

('Mechanical Keyboard','Keychron','RGB Keyboard','Mechanical keyboard with RGB',NULL,119.99,250,25),

('27 inch Monitor','Dell','IPS Monitor','4K IPS monitor',15.00,399.99,120,15),

('USB-C Cable','Anker','2m cable','USB-C fast charging cable',0.00,14.99,800,300),

('Office Chair','IKEA','Comfortable chair','Ergonomic office chair',20.00,249.99,60,8),

('Notebook','Oxford','Paper notebook','200 pages notebook',NULL,4.99,1500,500),

('Gaming Laptop','ASUS','RTX Laptop','High-end gaming laptop',5.00,1899.99,35,3),

('Desk Lamp','Philips','LED Lamp','LED office lamp',NULL,39.99,180,0);

-- =====================
-- PRODUCT CATEGORIES
-- =====================

INSERT INTO products_categories
VALUES
(1,1),
(1,3),

(2,2),

(3,1),
(3,2),

(4,1),

(5,5),

(6,4),

(7,1),
(7,2),
(7,3),

(8,5),
(8,6);

-- =====================
-- CARTS
-- =====================

INSERT INTO carts(user_id_fk)
VALUES
(1),
(4),
(6);

-- =====================
-- CART ITEMS
-- =====================

INSERT INTO carts_products
VALUES
(1,1,2),
(4,1,5),
(6,1,3),

(2,2,1),

(7,3,1),
(5,3,2);

-- =====================
-- ORDERS
-- =====================

INSERT INTO orders(order_status_id_fk,user_id_fk)
VALUES
(2,1), -- Paid
(3,1), -- Shipped
(4,4), -- Delivered
(5,6), -- Cancelled
(2,6);

-- =====================
-- ORDER PRODUCTS
-- =====================

INSERT INTO orders_products
VALUES
(1,1,1,'picked'),
(1,4,2,'packed'),

(2,7,1,'packed'),
(2,4,3,'packed'),

(3,5,1,'none'),
(3,6,5,'none'),

(4,3,1,'none'),

(5,2,1,'picked'),
(5,1,2,'none');

COMMIT;
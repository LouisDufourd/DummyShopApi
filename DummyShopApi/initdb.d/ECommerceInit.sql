drop table if exists carts_products;
drop table if exists carts;
drop table if exists orders_products;
drop table if exists orders;
drop table if exists orders_status;
drop table if exists images;
drop table if exists products_categories;
drop table if exists categories;
drop table if exists products;
drop table if exists users;
drop table if exists addresses;
drop extension if exists unaccent;

create extension unaccent;

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
	email						varchar(150) 	not null,
	password					varchar(255)	not null,
	created_at					timestamp 		not null	default now(),
	admin						bool			not null,
	email_verification_token 	varchar(255),
	email_verified				bool			not null	default false
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
	order_status_id		serial4  		not null	primary key,
	label				varchar(100) 	not null
);

create table orders(
	order_id			serial4			not null 	primary key,
	order_status_id_fk	int4			not null	references orders_status(order_status_id),
	user_id_fk			int4			not null	references users(user_id),
	created_at			timestamp		not null	default now()
);

create table orders_products(
	order_id_fk			int4 			not null	references orders(order_id),
	product_id_fk		int4			not null	references products(product_id),
	quantity			int4			not null,
	constraint pk_orders_product primary key (order_id_fk, product_id_fk)
);

create table carts(
	cart_id 			serial4 		not null	primary key,
	user_id_fk			int4			not null 	references users(user_id)	unique,
	created_at			timestamp		not null	default now()
);

create table carts_products(
	product_id_fk		int4 			not null 	references products(product_id),
	cart_id_fk			int4 			not null 	references carts(cart_id),
	quantity			int4 			not null,
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

-- 1) orders_status
INSERT INTO orders_status(label) VALUES
('Panier'),
('Payée'),
('Expédiée'),
('Livrée'),
('Annulée');

-- 2) categories
INSERT INTO categories(name, description) VALUES
('Informatique', 'Matériel informatique et périphériques'),
('Gaming', 'Accessoires orientés jeu'),
('Audio', 'Casques, micros et audio'),
('Bureau', 'Mobilier et accessoires de bureau'),
('Mobile', 'Accessoires smartphones/tablettes'),
('Stockage', 'SSD, HDD et stockage'),
('Réseau', 'Routeurs, switch et câbles'),
('Maison', 'Objets pratiques pour la maison');

-- 3) products (30)
INSERT INTO products(name, seller, short_desc, description, discount, price, sells_score, quantity, created_at) VALUES
('Clavier mécanique 75%', 'KeyFactory', 'RGB, switches linéaires', 'Clavier 75% aluminium, keycaps PBT, RGB.', 10.00, 99.99, 540, 40, now() - interval '50 days'),
('Souris gaming 26K DPI', 'ProMouse', '58g, capteur précis', 'Souris légère, 26K DPI, faible latence.', NULL, 59.90, 720, 120, now() - interval '49 days'),
('Casque fermé + micro', 'SoundPeak', 'Micro détachable', 'Casque fermé confortable, micro détachable.', 5.00, 79.00, 410, 60, now() - interval '47 days'),
('Écran 27" QHD 165Hz', 'ViewMax', 'IPS, VRR', 'Écran 1440p 165Hz, IPS, VRR compatible.', 40.00, 299.99, 260, 18, now() - interval '46 days'),
('SSD NVMe 1To Gen4', 'FlashCore', '7000MB/s', 'SSD NVMe Gen4 rapide pour OS/jeux.', 15.00, 119.99, 980, 90, now() - interval '45 days'),
('SSD NVMe 2To Gen4', 'FlashCore', '7400MB/s', 'SSD NVMe Gen4 2To, idéal gros jeux.', 25.00, 209.99, 610, 55, now() - interval '44 days'),
('Clé USB 128Go', 'Memoria', 'USB 3.2', 'Clé USB 3.2 compacte et rapide.', NULL, 12.99, 1500, 400, now() - interval '43 days'),
('Hub USB-C 8-en-1', 'DockEase', 'HDMI + Ethernet', 'Hub USB-C HDMI 4K, Ethernet, SD, USB.', NULL, 44.50, 690, 140, now() - interval '41 days'),
('Webcam 1080p', 'CamPro', 'Autofocus', 'Webcam 1080p autofocus, micro stéréo.', 7.00, 49.99, 350, 80, now() - interval '40 days'),
('Micro USB cardioïde', 'VocalLab', 'Idéal streaming', 'Micro cardioïde USB, anti-pop inclus.', 10.00, 69.90, 280, 45, now() - interval '39 days'),
('Tapis de souris XL', 'DeskGear', '900x400', 'Tapis XL antidérapant, couture renforcée.', 3.00, 19.90, 1200, 220, now() - interval '38 days'),
('Lampe de bureau LED', 'BrightHome', 'Température réglable', 'Lampe LED, intensité + température réglables.', NULL, 24.99, 900, 260, now() - interval '37 days'),
('Chaise ergonomique', 'ErgoSeat', 'Support lombaire', 'Chaise réglable, support lombaire, accoudoirs 3D.', 30.00, 199.00, 160, 14, now() - interval '36 days'),
('Support écran', 'ErgoDesk', 'Réglable', 'Support écran réglable, rangement câbles.', NULL, 29.90, 410, 70, now() - interval '35 days'),
('Routeur Wi-Fi 6', 'NetWave', 'AX3000', 'Routeur Wi-Fi 6 stable et rapide.', 20.00, 109.90, 240, 30, now() - interval '34 days'),
('Switch 8 ports', 'NetWave', 'Gigabit', 'Switch 8 ports gigabit plug & play.', NULL, 24.90, 530, 95, now() - interval '33 days'),
('Câble Ethernet 10m', 'CableLab', 'Cat6', 'Câble Cat6 10m, connecteurs renforcés.', NULL, 11.90, 800, 300, now() - interval '32 days'),
('Batterie externe 20 000mAh', 'Voltify', 'USB-C PD 20W', 'Powerbank 20k, charge rapide USB-C.', NULL, 34.90, 1300, 200, now() - interval '31 days'),
('Chargeur USB-C 65W', 'Voltify', 'GaN', 'Chargeur GaN 65W pour laptop et mobile.', 5.00, 39.90, 840, 110, now() - interval '30 days'),
('Coque téléphone', 'Case&Co', 'Antichoc', 'Coque antichoc coins renforcés.', NULL, 14.90, 2100, 500, now() - interval '29 days'),
('Verre trempé (x2)', 'Case&Co', 'Pack x2', 'Protections écran en verre trempé, pack 2.', NULL, 9.90, 3100, 800, now() - interval '28 days'),
('Enceinte Bluetooth', 'SoundPeak', 'Autonomie 12h', 'Enceinte compacte, 12h, basses correctes.', 8.00, 49.90, 600, 75, now() - interval '27 days'),
('Support téléphone', 'BrightHome', 'Bureau', 'Support téléphone bureau, angle réglable.', NULL, 7.90, 1800, 700, now() - interval '26 days'),
('Sac à dos laptop 15"', 'UrbanCarry', 'Imperméable', 'Sac laptop 15", compartiments, imperméable.', 12.00, 54.90, 340, 40, now() - interval '25 days'),
('Multiprise parafoudre', 'BrightHome', '6 prises', 'Multiprise parafoudre + USB.', NULL, 22.90, 460, 85, now() - interval '24 days'),
('Câble USB-C 2m', 'CableLab', 'Nylon', 'Câble USB-C 2m tressé, charge rapide.', NULL, 9.99, 5000, 1200, now() - interval '23 days'),
('Ventilateur PC 120mm', 'AirFlow', 'Silencieux', 'Ventilateur 120mm silencieux, bon airflow.', NULL, 8.90, 950, 350, now() - interval '22 days'),
('Pâte thermique', 'AirFlow', '4g', 'Pâte thermique 4g pour CPU/GPU.', NULL, 6.90, 1400, 600, now() - interval '21 days'),
('Kit nettoyage écran', 'BrightHome', 'Spray + chiffon', 'Spray + microfibre pour écrans.', NULL, 5.90, 1700, 900, now() - interval '20 days'),
('Cahier notes A5', 'Paper&Co', '200 pages', 'Cahier A5 200 pages, papier épais.', NULL, 4.50, 600, 400, now() - interval '19 days');

-- 4) products_categories (mapping logique)
INSERT INTO products_categories(product_id_fk, category_id_fk) VALUES
(1,1),(1,2),
(2,1),(2,2),
(3,3),(3,2),
(4,1),(4,2),
(5,6),(5,1),
(6,6),(6,1),
(7,6),
(8,1),(8,5),
(9,1),
(10,3),
(11,2),(11,4),
(12,4),
(13,4),
(14,4),
(15,7),
(16,7),
(17,7),
(18,5),
(19,5),
(20,5),
(21,5),
(22,3),(22,8),
(23,5),(23,4),
(24,8),
(25,8),
(26,5),
(27,1),
(28,1),
(29,1),
(30,4);

-- 5) images (3 par produit)
INSERT INTO images(product_id_fk, url, alt, created_at)
SELECT
  p.product_id,
  format('https://picsum.photos/seed/p%1$s_%2$s/900/700', p.product_id, i.n),
  format('%s - image %s', p.name, i.n),
  p.created_at + (i.n || ' days')::interval
FROM products p
CROSS JOIN (VALUES (1),(2),(3)) AS i(n);

-- 6) addresses (20)
INSERT INTO addresses(street_number, street_name, city, postcode, country, created_at) VALUES
('12','Rue de Rivoli','Paris','75001','France', now() - interval '60 days'),
('8','Avenue des Champs-Élysées','Paris','75008','France', now() - interval '58 days'),
('24','Rue Nationale','Lille','59800','France', now() - interval '55 days'),
('3','Rue Sainte-Catherine','Bordeaux','33000','France', now() - interval '53 days'),
('15','Rue de la République','Lyon','69002','France', now() - interval '50 days'),
('6','Place du Capitole','Toulouse','31000','France', now() - interval '49 days'),
('41','Quai de la Fosse','Nantes','44000','France', now() - interval '47 days'),
('9','Rue Paradis','Marseille','13006','France', now() - interval '46 days'),
('7','Rue Jean Jaurès','Rennes','35000','France', now() - interval '44 days'),
('18','Rue de la Paix','Nice','06000','France', now() - interval '43 days'),
('5','Boulevard Victor Hugo','Montpellier','34000','France', now() - interval '42 days'),
('22','Rue du Dôme','Strasbourg','67000','France', now() - interval '41 days'),
('10','Rue de la Liberté','Dijon','21000','France', now() - interval '40 days'),
('2','Rue de Siam','Brest','29200','France', now() - interval '39 days'),
('14','Rue du Port','Le Havre','76600','France', now() - interval '38 days'),
('19','Rue d’Alsace','Reims','51100','France', now() - interval '37 days'),
('1','Rue Royale','Orléans','45000','France', now() - interval '36 days'),
('11','Rue des Fleurs','Grenoble','38000','France', now() - interval '35 days'),
('27','Rue du Théâtre','Clermont-Ferrand','63000','France', now() - interval '34 days'),
('33','Rue du Vieux Marché','Rouen','76000','France', now() - interval '33 days');

-- 7) users (20) (FK address_id_fk ok)
INSERT INTO users(address_id_fk, name, firstname, email, password, created_at, admin) VALUES
(1, 'Durand','Léa','lea.durand@example.com', '$2b$10$fakehash_lea', now() - interval '55 days', false),
(2, 'Martin','Hugo','hugo.martin@example.com', '$2b$10$fakehash_hugo', now() - interval '53 days', false),
(3, 'Bernard','Inès','ines.bernard@example.com', '$2b$10$fakehash_ines', now() - interval '50 days', false),
(4, 'Petit','Lucas','lucas.petit@example.com', '$2b$10$fakehash_lucas', now() - interval '49 days', false),
(5, 'Robert','Emma','emma.robert@example.com', '$2b$10$fakehash_emma', now() - interval '47 days', false),
(6, 'Richard','Noah','noah.richard@example.com', '$2b$10$fakehash_noah', now() - interval '46 days', false),
(7, 'Moreau','Chloé','chloe.moreau@example.com', '$2b$10$fakehash_chloe', now() - interval '44 days', false),
(8, 'Fournier','Louis','louis.fournier@example.com', '$2b$10$fakehash_louis', now() - interval '43 days', false),
(9, 'Girard','Jade','jade.girard@example.com', '$2b$10$fakehash_jade', now() - interval '42 days', false),
(10,'Andre','Liam','liam.andre@example.com', '$2b$10$fakehash_liam', now() - interval '41 days', false),
(11,'Admin','Shop','admin@shop.local', '$2b$10$fakehash_admin', now() - interval '100 days', true),
(12,'Lopez','Manon','manon.lopez@example.com', '$2b$10$fakehash_manon', now() - interval '40 days', false),
(13,'Nguyen','Adam','adam.nguyen@example.com', '$2b$10$fakehash_adam', now() - interval '39 days', false),
(14,'Lefevre','Zoé','zoe.lefevre@example.com', '$2b$10$fakehash_zoe', now() - interval '38 days', false),
(15,'Dubois','Lina','lina.dubois@example.com', '$2b$10$fakehash_lina', now() - interval '37 days', false),
(16,'Roux','Maël','mael.roux@example.com', '$2b$10$fakehash_mael', now() - interval '36 days', false),
(17,'Vincent','Sarah','sarah.vincent@example.com', '$2b$10$fakehash_sarah', now() - interval '35 days', false),
(18,'Mercier','Enzo','enzo.mercier@example.com', 'AQAAAAIAAYagAAAAEGrWRdXQmzg20IcycksgJUOdeSCyV3szr+KMACPJ29Zg+PR2Snj9awzyidLP/ts0jg==', now() - interval '34 days', false),
(19,'Garnier','Anaïs','anais.garnier@example.com', '+PBDQsv97atRiMJe7CGP392tzYRxZT4V/SASSO52hDgw==', now() - interval '33 days', false),
(20,'Faure','Tom','tom.faure@example.com', '+oJkCXN8oVewacnCTQ3meumMMXZ20Zna34vKV7Vn8zqpQ==', now() - interval '32 days', false);

-- 8) carts (1 par user) + user_id_fk UNIQUE respecté
INSERT INTO carts(user_id_fk, created_at)
SELECT u.user_id, u.created_at + interval '1 day'
FROM users u;

-- 9) carts_products (chaque user a 2-4 items)
INSERT INTO carts_products(product_id_fk, cart_id_fk, quantity) VALUES
-- user 1 => cart 1
(1, 1, 1),(26, 1, 2),(11, 1, 1),
-- user 2 => cart 2
(2, 2, 1),(21, 2, 1),(27, 2, 2),
-- user 3 => cart 3
(4, 3, 1),(5, 3, 1),
-- user 4 => cart 4
(13, 4, 1),(12, 4, 1),(30, 4, 2),
-- user 5 => cart 5
(18, 5, 1),(20, 5, 1),(21, 5, 2),
-- user 6 => cart 6
(8, 6, 1),(9, 6, 1),
-- user 7 => cart 7
(6, 7, 1),(25, 7, 1),(29, 7, 1),
-- user 8 => cart 8
(3, 8, 1),(10, 8, 1),
-- user 9 => cart 9
(14, 9, 1),(24, 9, 1),(26, 9, 3),
-- user 10 => cart 10
(7, 10, 2),(11, 10, 1),(21, 10, 2),
-- user 11 => cart 11 (admin)
(15, 11, 1),(16, 11, 1),
-- user 12 => cart 12
(5, 12, 1),(8, 12, 1),
-- user 13 => cart 13
(19, 13, 1),(12, 13, 1),(23, 13, 2),
-- user 14 => cart 14
(4, 14, 1),(1, 14, 1),
-- user 15 => cart 15
(20, 15, 2),(21, 15, 2),
-- user 16 => cart 16
(9, 16, 1),(25, 16, 1),
-- user 17 => cart 17
(2, 17, 1),(3, 17, 1),
-- user 18 => cart 18
(6, 18, 1),(18, 18, 1),
-- user 19 => cart 19
(10, 19, 1),(30, 19, 3),
-- user 20 => cart 20
(11, 20, 2),(26, 20, 1);

-- 10) orders (12 commandes) avec statuts variés
-- status ids (selon insertion): 1 Panier, 2 Payée, 3 Expédiée, 4 Livrée, 5 Annulée
INSERT INTO orders(order_status_id_fk, user_id_fk, created_at) VALUES
(2, 2,  now() - interval '20 days'),
(3, 3,  now() - interval '18 days'),
(4, 4,  now() - interval '15 days'),
(2, 5,  now() - interval '14 days'),
(5, 6,  now() - interval '13 days'),
(4, 7,  now() - interval '12 days'),
(3, 8,  now() - interval '10 days'),
(2, 9,  now() - interval '9 days'),
(4, 10, now() - interval '8 days'),
(2, 12, now() - interval '7 days'),
(3, 14, now() - interval '6 days'),
(4, 15, now() - interval '5 days');

-- 11) orders_products (lignes par commande)
INSERT INTO orders_products(order_id_fk, product_id_fk, quantity) VALUES
-- order 1 (user 2) : écran + ssd
(1, 4, 1),
(1, 5, 1),

-- order 2 (user 3) : casque + micro
(2, 3, 1),
(2, 10, 1),

-- order 3 (user 4) : chaise + lampe
(3, 13, 1),
(3, 12, 1),

-- order 4 (user 5) : powerbank + chargeur + câble
(4, 18, 1),
(4, 19, 1),
(4, 26, 2),

-- order 5 (user 6) : annulée
(5, 15, 1),
(5, 16, 1),

-- order 6 (user 7) : livré
(6, 1, 1),
(6, 2, 1),
(6, 11, 1),

-- order 7 (user 8) : expédiée
(7, 22, 1),
(7, 24, 1),

-- order 8 (user 9) : payée
(8, 20, 1),
(8, 21, 1),

-- order 9 (user 10) : livré
(9, 6, 1),
(9, 8, 1),

-- order 10 (user 12) : payée
(10, 5, 1),
(10, 7, 2),

-- order 11 (user 14) : expédiée
(11, 4, 1),
(11, 26, 1),

-- order 12 (user 15) : livré
(12, 9, 1),
(12, 14, 1);

COMMIT;
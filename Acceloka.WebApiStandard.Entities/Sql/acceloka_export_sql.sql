--
-- PostgreSQL database dump
--

\restrict ttakfm9aRjus5gHEXCkwUpyw3b6PNQVFLiK7RnlkeoBcdFTlPgTk5m2DhtXROPZ

-- Dumped from database version 17.5
-- Dumped by pg_dump version 18.0

-- Started on 2026-02-12 22:32:14

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 222 (class 1259 OID 203085)
-- Name: booking_tickets; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.booking_tickets (
    booking_id integer NOT NULL,
    ticket_code character varying(10) NOT NULL,
    quantity integer DEFAULT 1 NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by character varying(255) DEFAULT 'System'::character varying NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by character varying(255) DEFAULT 'System'::character varying NOT NULL,
    CONSTRAINT booking_tickets_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE public.booking_tickets OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 203072)
-- Name: bookings; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.bookings (
    id integer NOT NULL,
    booking_date timestamp with time zone DEFAULT now() NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by character varying(255) DEFAULT 'System'::character varying NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by character varying(255) DEFAULT 'System'::character varying NOT NULL
);


ALTER TABLE public.bookings OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 203071)
-- Name: bookings_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.bookings_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.bookings_id_seq OWNER TO postgres;

--
-- TOC entry 4944 (class 0 OID 0)
-- Dependencies: 220
-- Name: bookings_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.bookings_id_seq OWNED BY public.bookings.id;


--
-- TOC entry 218 (class 1259 OID 203042)
-- Name: categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.categories (
    id integer NOT NULL,
    name character varying(255) NOT NULL,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by character varying(255) DEFAULT 'System'::character varying NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by character varying(255) DEFAULT 'System'::character varying NOT NULL
);


ALTER TABLE public.categories OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 203041)
-- Name: categories_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.categories_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.categories_id_seq OWNER TO postgres;

--
-- TOC entry 4945 (class 0 OID 0)
-- Dependencies: 217
-- Name: categories_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.categories_id_seq OWNED BY public.categories.id;


--
-- TOC entry 219 (class 1259 OID 203054)
-- Name: tickets; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tickets (
    ticket_code character varying(10) NOT NULL,
    name character varying(255) NOT NULL,
    event_date timestamp with time zone NOT NULL,
    price numeric(10,2) NOT NULL,
    quota integer NOT NULL,
    category_id integer,
    created_at timestamp with time zone DEFAULT now() NOT NULL,
    created_by character varying(255) DEFAULT 'System'::character varying NOT NULL,
    updated_at timestamp with time zone DEFAULT now() NOT NULL,
    updated_by character varying(255) DEFAULT 'System'::character varying NOT NULL,
    CONSTRAINT tickets_quota_check CHECK ((quota >= 0))
);


ALTER TABLE public.tickets OWNER TO postgres;

--
-- TOC entry 4764 (class 2604 OID 203075)
-- Name: bookings id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.bookings ALTER COLUMN id SET DEFAULT nextval('public.bookings_id_seq'::regclass);


--
-- TOC entry 4755 (class 2604 OID 203045)
-- Name: categories id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories ALTER COLUMN id SET DEFAULT nextval('public.categories_id_seq'::regclass);


--
-- TOC entry 4938 (class 0 OID 203085)
-- Dependencies: 222
-- Data for Name: booking_tickets; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.booking_tickets (booking_id, ticket_code, quantity, created_at, created_by, updated_at, updated_by) FROM stdin;
1	TD001	1	2026-02-10 04:41:14.321492+07	system	2026-02-10 04:41:14.321492+07	system
1	TL001	2	2026-02-10 04:41:14.321492+07	system	2026-02-10 04:41:14.321492+07	system
4	HO001	30	2026-02-12 23:22:02.390489+07	System	2026-02-12 23:22:02.390489+07	System
4	TL001	10	2026-02-12 23:22:02.390489+07	System	2026-02-12 23:22:02.390489+07	System
5	HO001	30	2026-02-12 21:10:45.623277+07	System	2026-02-12 21:10:45.623277+07	System
5	TL001	20	2026-02-12 21:10:45.623277+07	System	2026-02-12 21:10:45.623277+07	System
6	HO001	11	2026-02-12 21:36:41.479131+07	System	2026-02-12 21:36:41.479131+07	System
6	TL001	10	2026-02-12 21:36:41.479131+07	System	2026-02-12 21:36:41.479131+07	System
2	CO001	2	2026-02-11 21:02:22.560276+07	System	2026-02-11 21:02:22.560276+07	System
2	HO001	1	2026-02-11 21:02:22.560276+07	System	2026-02-11 21:02:22.560276+07	System
\.


--
-- TOC entry 4937 (class 0 OID 203072)
-- Dependencies: 221
-- Data for Name: bookings; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.bookings (id, booking_date, created_at, created_by, updated_at, updated_by) FROM stdin;
1	2026-02-10 04:41:14.321492+07	2026-02-10 04:41:14.321492+07	system	2026-02-10 04:41:14.321492+07	system
2	2026-02-11 21:02:20.565633+07	2026-02-11 21:02:22.560276+07	System	2026-02-11 21:02:22.560276+07	System
4	2026-02-12 23:22:02.041514+07	2026-02-12 23:22:02.390489+07	System	2026-02-12 23:22:02.390489+07	System
5	2026-02-12 21:10:45.145645+07	2026-02-12 21:10:45.623277+07	System	2026-02-12 21:10:45.623277+07	System
6	2026-02-12 21:36:41.399718+07	2026-02-12 21:36:41.479131+07	System	2026-02-12 21:36:41.479131+07	System
\.


--
-- TOC entry 4934 (class 0 OID 203042)
-- Dependencies: 218
-- Data for Name: categories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.categories (id, name, created_at, created_by, updated_at, updated_by) FROM stdin;
1	Hotel	2026-02-09 16:00:11.252479+07	System	2026-02-09 16:00:11.252479+07	System
2	Transportasi Darat	2026-02-09 16:00:11.252479+07	System	2026-02-09 16:00:11.252479+07	System
3	Transportasi Laut	2026-02-09 16:00:11.252479+07	System	2026-02-09 16:00:11.252479+07	System
4	Cinema	2026-02-09 16:00:11.252479+07	System	2026-02-09 16:00:11.252479+07	System
\.


--
-- TOC entry 4935 (class 0 OID 203054)
-- Dependencies: 219
-- Data for Name: tickets; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tickets (ticket_code, name, event_date, price, quota, category_id, created_at, created_by, updated_at, updated_by) FROM stdin;
TD001	Bus jawa-sumatra	2027-04-03 00:59:00+07	50000000.00	79	2	2026-02-09 16:01:58.785128+07	System	2026-02-09 16:01:58.785128+07	System
TL001	Kapal Ferri jawa-sumatra	2027-04-03 00:59:00+07	50000000.00	28	3	2026-02-09 16:01:58.785128+07	System	2026-02-09 16:01:58.785128+07	System
TL002	Kapal Ferri jawa-Bali	2027-04-03 00:59:00+07	50000000.00	58	3	2026-02-09 16:01:58.785128+07	System	2026-02-09 16:01:58.785128+07	System
CO001	Ironman CGV	2027-04-03 00:59:00+07	50000000.00	77	4	2026-02-09 16:01:58.785128+07	System	2026-02-09 16:01:58.785128+07	System
HO001	Ibis Hotel Jakarta 21-23	2027-03-03 00:59:00+07	50000000.00	24	1	2026-02-09 16:01:58.785128+07	System	2026-02-09 16:01:58.785128+07	System
\.


--
-- TOC entry 4946 (class 0 OID 0)
-- Dependencies: 220
-- Name: bookings_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.bookings_id_seq', 6, true);


--
-- TOC entry 4947 (class 0 OID 0)
-- Dependencies: 217
-- Name: categories_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.categories_id_seq', 4, true);


--
-- TOC entry 4784 (class 2606 OID 203097)
-- Name: booking_tickets booking_tickets_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.booking_tickets
    ADD CONSTRAINT booking_tickets_pkey PRIMARY KEY (booking_id, ticket_code);


--
-- TOC entry 4782 (class 2606 OID 203084)
-- Name: bookings bookings_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.bookings
    ADD CONSTRAINT bookings_pkey PRIMARY KEY (id);


--
-- TOC entry 4778 (class 2606 OID 203053)
-- Name: categories categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.categories
    ADD CONSTRAINT categories_pkey PRIMARY KEY (id);


--
-- TOC entry 4780 (class 2606 OID 203065)
-- Name: tickets tickets_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_pkey PRIMARY KEY (ticket_code);


--
-- TOC entry 4786 (class 2606 OID 203098)
-- Name: booking_tickets booking_tickets_booking_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.booking_tickets
    ADD CONSTRAINT booking_tickets_booking_id_fkey FOREIGN KEY (booking_id) REFERENCES public.bookings(id) ON DELETE CASCADE;


--
-- TOC entry 4787 (class 2606 OID 203103)
-- Name: booking_tickets booking_tickets_ticket_code_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.booking_tickets
    ADD CONSTRAINT booking_tickets_ticket_code_fkey FOREIGN KEY (ticket_code) REFERENCES public.tickets(ticket_code);


--
-- TOC entry 4785 (class 2606 OID 203066)
-- Name: tickets tickets_category_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tickets
    ADD CONSTRAINT tickets_category_id_fkey FOREIGN KEY (category_id) REFERENCES public.categories(id);


-- Completed on 2026-02-12 22:32:14

--
-- PostgreSQL database dump complete
--

\unrestrict ttakfm9aRjus5gHEXCkwUpyw3b6PNQVFLiK7RnlkeoBcdFTlPgTk5m2DhtXROPZ

